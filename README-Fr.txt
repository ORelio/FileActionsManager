=============================================================
==== FileActionsManager v1.0 - Par ORelio - Microzoom.fr ====
=============================================================

Merci d'avoir t�l�charg� FileActionsManager !

Ce programme permet de cr�er des actions dans le menu contextuel de l'explorateur de fichiers
de Windows en utilisant des fichiers de configuration. Le but principal de FileActionsManager
est de faciliter l'installation des actions et fichiers associ�s sur plusieurs machines.

Si vous pr�f�rez configurer manuellement les actions des types de fichiers via une
interface facile � utiliser, le programme FileTypesMan propos� par Nirsoft sera
plus appropri� : https://www.nirsoft.net/utils/file_types_manager.html

=============
 Utilisation
=============

Lorsque vous ouvrez un fichier de configuration avec FileActionsManager, le programme vous
proposera d'installer l'action, ou de la d�sinstaller si elle existe d�j�.
Sinon, FileActionsManager permet d'associer ou d�sassicier les fichiers ".seinf".

===================================
 Cr�er un fichier de configuration
===================================

FileActionsManager utilise des fichiers texte au format INI portant l'extension ".seinf"
L'exemple ci-dessous permet d'optimiser vos images avec with RIOT (http://luci.criosweb.ro/riot/):

[ShellExtension]
Ext=bmp,png,gif,jpg
Name=optimizeimagesize
DisplayName=Optimiser la taille
Command=Riot.exe "%1"
Requires=Riot.exe,FreeImage.dll
Default=false

Chaque champ a la fonction suivante :

 Ext           Une liste d'extensions s�par�es par des virgules pour lesquelles cr�er l'action.
 Name          Nom interne de l'action, doit �tre unique pour ne pas �craser une autre action.
 DisplayName   Le texte � afficher dans le menu contextuel dans l'explorateur Windows.
 Command       La commande associ�e avec l'action, comme elle serait saisie dans la console CMD.
 Requires      Optionnel. Fichiers requis pour le bon fonctionnement de l'action. Voir ci-dessous.
 Default       Optionnel. D�finit si l'action sera l'action par d�faut (valeur par d�faut: false).
 
======================================
 Ajouter des fichiers compl�mentaires
======================================

En utilisant la fonction "Requires", FileActionsManager va rechercher les fichiers requis dans
le m�me dossier que votre fichier ".seinf". Par exemple, l'architecture suivante est valide :

OptimiserImages
 |- OptimiserImages.seinf
 |- FreeImage.dll
 `- Riot.exe

En ouvrant"OptimiserImages.seinf", FileActionsManager va copier "FreeImage.dll"
et "Riot.exe" dans "%appdata%\ShellExtensions". De plus, le champ "Command" sera
adapt� pour utiliser le chemin complet vers "Riot.exe" avant d'�tre �crit dans le registre.

Il est possible de partager un m�me ex�cutable entre deux actions, par exemple il est possible
de fournir "ffmpeg.exe" � la fois avec "ConvertirMP3.seinf" et "ConvertirAAC.seinf".
"ffmpeg.exe" sera plac� dans "%appdata%\ShellExtensions", en tenant compte des actions
d�pendant de "ffmpeg.exe". L'ex�cutable ne sera supprim� que lorsque la derni�re action
qui en d�pend est supprim�e.

==================================
 Utilisation en ligne de commande
==================================

Comme alternative aux fichiers de configuration, FileActionsConsole peut �tre utilis� :

FileActionsConsole.exe add <extension(s)> <nom interne> <texte � afficher> <commande> [defaut=false]
FileActionsConsole.exe del <extension(s)> <nom interne>

Exemple avec l'action "Optimiser la taille" :

FileActionsConsole.exe add bmp,png,gif,jpg optimizeimagesize "Optimiser la taille" "Riot.exe "%1""
FileActionsConsole.exe del bmp,png,gif,jpg optimizeimagesize

Cet outil ne g�re pas les fichiers compl�mentaires, il faudra donc les installer autrement.
Si vous n'en avez pas besoin, vous pouvez supprimer FileActionsConsole.exe sans souci.

=====
 FAQ
=====

Q: Lors de la cr�ation de certaines actions SANS le champ "Default=true",
   l'action devient quand m�me l'action par d�faut !
R: Certains types de fichiers dans Windows n'ont pas d'action par d�faut, et Windows prend la
   premi�re action de la liste. Il est possible de corriger cela en d�finissant manuellement
   une action par d�faut via le logiciel FileTypesMan de Nirsoft, ou bien en choisissant un
   nom interne du type zzmonaction avant cr�ation de sorte que celle-ci ne soit pas en
   premi�re position dans la liste et donc ne soit pas choisie comme action par d�faut.

Q: Que se passe-t-il si deux actions diff�rentes sont fournies avec deux fichiers
   de m�me nom, par exemple des ex�cutables, mais dont le contenu est diff�rent ?
R: Cela n'est pas g�r� et un seul des deux fichiers sera copi� dans AppData.

Q: Comment d�sinstaller une action ?
R: Ouvrez de nouveau le fichier ".seinf" avec FileActionsManager.

Q: Comment modifier une action ?
R: D�sinstallez-la, puis r�installez-la.
   Egalement, d�sinstallez-la *avant* de modifier son nom interne.

Q: O� puis-je obtenir le code source, contribuer et obtenir de l'aide ?
R: Si possible en anglais ici : https://github.com/ORelio/FileActionsManager/
   Sinon, en fran�ais sur mon site internet : https://microzoom.fr/

=========
 Cr�dits
=========

Les ic�nes suivantes sont utilis�es au sein de FileActionsManager :

 - Buuf icons par Mattahan: Text Editor icon
 - Buuf icons par Mattahan: Menu icon

Source: http://www.iconarchive.com/show/buuf-icons-by-mattahan/
Ces ic�nes sont licensi�es sous Creative Commons Attribution-Noncommercial-Share Alike 4.0.

+--------------------+
| � 2015-2018 ORelio |
+--------------------+