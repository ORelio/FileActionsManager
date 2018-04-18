using System;
using System.Linq;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;
using System.Collections.Generic;
using SharpTools;
using System.ComponentModel;
using System.Reflection;

namespace FileActionsManager
{
    /// <summary>
    /// Small utility for installing/uninstalling file context menu actions using a configuration file
    /// By ORelio - (c) 2015-2018 - Available under the CDDL-1.0 license
    /// </summary>
    static class Program
    {
        private static string AppData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        private static string AppFolder = AppData + "\\ShellExtensions";
        private static string AppDepsCacheFile = AppFolder + "\\deps.ini";
        private static string AppDepsSection = "dependencies";

        public const string AppVer = "1.0";
        public static readonly string AppName = typeof(Program).Assembly.GetName().Name;
        public static readonly string AppDesc = AppName + " v" + AppVer;

        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            try
            {
                if (args.Length > 0 && !args[0].StartsWith("--"))
                {
                    if (File.Exists(args[0]))
                    {
                        try
                        {
                            var cfgFile = INIFile.ParseFile(args[0]);
                            if (cfgFile != null && cfgFile.ContainsKey("shellextension"))
                            {
                                var settings = cfgFile["shellextension"];
                                string[] required = new[] { "ext", "name", "displayname", "command" };
                                if (required.All(setting => settings.ContainsKey(setting)))
                                {
                                    string[] extensions = settings["ext"].Split(',');
                                    string actionName = settings["name"];
                                    string displayName = settings["displayname"];
                                    string command = settings["command"];
                                    string[] dependencies = settings.ContainsKey("requires") ?
                                        settings["requires"].Split(new string[] { "," },
                                        StringSplitOptions.RemoveEmptyEntries) : new string[0];
                                    bool defaultAction = settings.ContainsKey("default") ?
                                        settings["default"].ToLower() == "true" : false;

                                    if (ShellFileType.ActionExistsAny(extensions, actionName))
                                    {
                                        if ((args.Contains("--yes") || MessageBox.Show("Action '" + displayName + "' already exists.\nUninstall?",
                                            AppDesc, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes))
                                        {
                                            if (dependencies.Length > 0)
                                                HandleDependencies(dependencies, args[0], actionName, displayName, ref command, false);
                                            ShellFileType.RemoveAction(extensions, actionName);
                                            MessageBox.Show("Action '" + displayName + "' has been successfully removed.",
                                                AppDesc, MessageBoxButtons.OK, MessageBoxIcon.Information);
                                        }
                                    }

                                    else if (args.Contains("--yes") || MessageBox.Show(
                                        "Install action '" + displayName + "' to the following file extensions?\n"
                                        + String.Join(", ", extensions), AppDesc, MessageBoxButtons.YesNo,
                                            MessageBoxIcon.Question) == DialogResult.Yes)
                                    {
                                        if (dependencies.Length > 0
                                            && !HandleDependencies(dependencies, args[0], actionName, displayName, ref command, true))
                                            return;
                                        ShellFileType.AddAction(extensions, actionName, displayName, command, defaultAction);
                                        MessageBox.Show("Action '" + displayName + "' has been successfully installed.",
                                            AppDesc, MessageBoxButtons.OK, MessageBoxIcon.Information);
                                    }
                                }
                                else MessageBox.Show("File '" + Path.GetFileName(args[0]) + "' has missing required fields: "
                                    + String.Join(", ", required.Where(setting => !settings.ContainsKey(setting)).ToArray()),
                                        AppDesc, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                            }
                            else MessageBox.Show("File '" + Path.GetFileName(args[0]) + "' is not a valid shell extension file.",
                                AppDesc, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        }
                        catch (IOException)
                        {
                            MessageBox.Show("Cannot read '" + Path.GetFileName(args[0]) + "'.",
                                AppDesc, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        }
                    }
                    else MessageBox.Show("Cannot find '" + Path.GetFileName(args[0]) + "'.",
                        AppDesc, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
                else
                {
                    if (!args.Contains("--install")
                        && (args.Contains("--uninstall") || ShellFileType.ActionExists("seinf", "open")))
                    {
                        if (args.Contains("--yes") || MessageBox.Show(AppName
                            + " is currently associated with .seinf files.\nRemove association?",
                                AppDesc, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                        {
                            args = new string[] { "--uninstall" };
                            using (ShellFileType seinf = ShellFileType.GetType("seinf"))
                            {
                                seinf.ProgId = null;
                            }
                            MessageBox.Show("File association has been removed.", AppDesc,
                                MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                    else
                    {
                        if (args.Contains("--yes") || MessageBox.Show(AppName
                            + " is currently not associated with .seinf files. Associate?",
                                AppDesc, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                        {
                            args = new string[] { "--install" };
                            using (ShellFileType seinf = ShellFileType.GetOrCreateType("seinf"))
                            {
                                seinf.ProgId = AppName + ".File";
                                seinf.Description = "Shell Extension Information File";
                                seinf.DefaultIcon = Assembly.GetEntryAssembly().Location + ",1";
                                seinf.DefaultAction = "open";
                                seinf.MenuItems.Clear();
                                seinf.MenuItems.Add("open",
                                    new ShellFileType.MenuItem("&Install", "\"" + Assembly.GetEntryAssembly().Location + "\" \"%1\""));
                                seinf.MenuItems.Add("edit",
                                    new ShellFileType.MenuItem("&Edit", "notepad \"%1\""));
                            }
                            MessageBox.Show("File association has been installed.", AppDesc,
                                MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString(), "Something went wrong!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Handle shell extension dependency files
        /// </summary>
        /// <param name="dependencies">List of files required by this shell extension</param>
        /// <param name="actionFile">File describing the shell extension</param>
        /// <param name="actionName">Internal name of the shell extension</param>
        /// <param name="displayName">Display name of the shell extension</param>
        /// <param name="command">Command of the shell extension</param>
        /// <param name="install">TRUE to INSTALL, FALSE to UNINSTALL the dependencies</param>
        /// <returns>True if the (un)install procedure successfuly completed</returns>
        private static bool HandleDependencies(
            string[] dependencies, string actionFile, string actionName,
            string displayName, ref string command, bool install)
        {
            //Create dependencies directory if first dependencies install
            if (install)
            {
                if (!Directory.Exists(AppFolder))
                    Directory.CreateDirectory(AppFolder);
            }

            //Load or dependencies dictionary
            var deps = new Dictionary<string, Dictionary<string, string>>();
            if (File.Exists(AppDepsCacheFile))
                deps = INIFile.ParseFile(AppDepsCacheFile);
            if (!deps.ContainsKey(AppDepsSection))
                deps[AppDepsSection] = new Dictionary<string, string>();

            foreach (string dep in dependencies)
            {
                //Install depencency
                if (install)
                {
                    if (File.Exists(AppFolder + dep)) { }
                    else if (File.Exists(Path.GetDirectoryName(actionFile) + '\\' + dep))
                        File.Copy(Path.GetDirectoryName(actionFile) + '\\' + dep, AppFolder + '\\' + dep, true);
                    else
                    {
                        MessageBox.Show("Action '" + displayName + "' requires '" + dep
                            + "', which is not installed and cannot be found.", AppDesc,
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return false;
                    }
                }

                //Update dependencies dictionary
                HashSet<string> depstmp = new HashSet<string>();
                if (deps[AppDepsSection].ContainsKey(dep))
                    foreach (string t in deps[AppDepsSection][dep].Split(','))
                        if (!depstmp.Contains(t))
                            depstmp.Add(t);
                if (install && !depstmp.Contains(actionName))
                    depstmp.Add(actionName);
                else if (!install)
                    depstmp.RemoveWhere(item => item == actionName);
                deps[AppDepsSection][dep]
                    = String.Join(",", depstmp.ToArray());

                //Remove dependency if no longer needed
                if (!install && String.IsNullOrEmpty(deps[AppDepsSection][dep]))
                {
                    File.Delete(AppFolder + '\\' + dep);
                    deps[AppDepsSection].Remove(dep);
                }

                //Auto-prepend app directory path to dependencies
                string[] commandSplitted = command.Split(' ');
                for (int i = 0; i < commandSplitted.Length; i++)
                    if (commandSplitted[i] == dep)
                        commandSplitted[i] = "\"" + AppFolder + '\\' + dep + '"';
                command = String.Join(" ", commandSplitted);
            }

            //Write back dependencies dictionary
            if (deps[AppDepsSection].Values.Count > 0)
                INIFile.WriteFile(AppDepsCacheFile, deps);
            else File.Delete(AppDepsCacheFile);

            //Remove dependencies directory if it's no longer required
            if (!install && !Directory.EnumerateFiles(AppFolder).Any())
                Directory.Delete(AppFolder, true);

            return true;
        }
    }
}
