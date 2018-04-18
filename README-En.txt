============================================================
==== FileActionsManager v1.0 - By ORelio - Microzoom.fr ====
============================================================

Thanks for dowloading FileActionsManager!

This program allows to create context menu actions in the Windows file explorer
using configuration files. The main purpose of FileActionsManager is to allow easy
installation of the action and associated standalone program, if any, on several machines.

If you are looking to manually configure file type actions using a nice GUI,
see FileTypesMan from Nirsoft instead: https://www.nirsoft.net/utils/file_types_manager.html

============
 How to use
============

When opening a configuration file with FileActionsManager, the program will offer to register
the provided context menu action, or unregister it if the action is already registered.
Otherwise, FileActionsManager offers to associate or unassociate itself with `.seinf` files. 

===========================
 Configuration file syntax
===========================

FileActionsManager works using INI files with the ".seinf" file extension.
The example below creates an action to optimize images with RIOT (http://luci.criosweb.ro/riot/):

[ShellExtension]
Ext=bmp,png,gif,jpg
Name=optimizeimagesize
DisplayName=Optimize image size
Command=Riot.exe "%1"
Requires=Riot.exe,FreeImage.dll
Default=false

Each field has the following purpose:

 Ext           A list of comma-separated file extensions for which the action needs to be created.
 Name          Internal name of the action, must be unique to avoid overwriting another action.
 DisplayName   The label displayed in file context menu inside Windows Explorer.
 Command       The command associated with the action, as typed in `cmd.exe` for instance.
 Requires      Optional. Files required for the action to work. See below for details.
 Default       Optional. Defines whether the action will become the default action (default: false).

============================
 Providing additional files
============================

When using the "Requires" feature, FileActionsManager will look for the required files
in the same directory as your ".seinf" file. For instance, the following architecture is valid:

OptimizeImages
 |- OptimizeImages.seinf
 |- FreeImage.dll
 `- Riot.exe

When opening "OptimizeImages.seinf", FileActionsManager will copy "FreeImage.dll"
and "Riot.exe" to "%appdata%\ShellExtensions". Furthermore, the command will
be adapted to the full path to "Riot.exe" before being set in registry.

It is possible to share the same executable between two actions, for instance it is possible
to provide "ffmpeg.exe" with both "ConvertMP3.seinf" and "ConvertAAC.seinf". "ffmpeg.exe" will be
stored in "%appdata%\ShellExtensions", keeping track of which actions depends "ffmpeg.exe".
The executable will be deleted only when the last action requiring it is removed.

====================
 Command-line usage
====================

Alternatively to configuration files, FileActionsConsole can be used to register and remove file actions:

FileActionsConsole.exe add <extension(s)> <internal name> <display name> <command> [default=false]
FileActionsConsole.exe del <extension(s)> <internal name>

Example with the "Optimize image size" action:

FileActionsConsole.exe add bmp,png,gif,jpg optimizeimagesize "Optimize image size" "Riot.exe "%1""
FileActionsConsole.exe del bmp,png,gif,jpg optimizeimagesize

The command-line utility does not support additional files, you'll have to install them by other means.
FileActionsConsole.exe is independent from FileActionsManager.exe so you can delete it if not needed.

=====
 FAQ
=====

Q: When registering some actions WITHOUT "Default=true", it is set as default anyway!
R: Some file types in Windows do not have a default action and Windows picks the first one.
   You can fix this by manually defining a default action using FileTypesMan from Nirsoft,
   or set the internal action name to something like zzmyaction before applying so that it does
   not become the first action in registry and does not get picked as the default action.

Q: What happens if two different actions use two different files with the same name?
R: Different files with the same name are not handled since only one version is stored.

Q: How to uninstall an action?
R: Open the ".seinf" again with FileActionsManager.

Q: How to change an action?
R: Uninstall it, then reinstall it.
   Also, uninstall it *before* changing its internal name.

Q: Where can I get source code, contribute or report issues?
R: Here: https://github.com/ORelio/FileActionsManager/

=========
 Credits
=========

The following icons are used within FileActionsManager:

 - Buuf icons by Mattahan: Text Editor icon
 - Buuf icons by Mattahan: Menu icon

Source: http://www.iconarchive.com/show/buuf-icons-by-mattahan/
These icons are licensed under Creative Commons Attribution-Noncommercial-Share Alike 4.0.

+--------------------+
| © 2015-2018 ORelio |
+--------------------+