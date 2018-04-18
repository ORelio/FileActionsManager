=============================================================
==== FileActionsManager v1.0 - Par ORelio - Microzoom.fr ====
=============================================================

Merci d'avoir téléchargé FileActionsManager !

Ce programme permet de créer des actions dans le menu contextuel de l'explorateur de fichiers
de Windows en utilisant des fichiers de configuration. Le but principal de FileActionsManager
est de faciliter l'installation des actions et fichiers associés sur plusieurs machines.

Si vous préférez configurer manuellement les actions des types de fichiers via une
interface facile à utiliser, le programme FileTypesMan proposé par Nirsoft sera
plus approprié : https://www.nirsoft.net/utils/file_types_manager.html

=============
 Utilisation
=============

Lorsque vous ouvrez un fichier de configuration avec FileActionsManager, le programme vous
proposera d'installer l'action, ou de la désinstaller si elle existe déjà.
Sinon, FileActionsManager permet d'associer ou désassicier les fichiers ".seinf".

===================================
 Créer un fichier de configuration
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

 Ext           Une liste d'extensions séparées par des virgules pour lesquelles créer l'action.
 Name          Nom interne de l'action, doit être unique pour ne pas écraser une autre action.
 DisplayName   Le texte à afficher dans le menu contextuel dans l'explorateur Windows.
 Command       La commande associée avec l'action, comme elle serait saisie dans la console CMD.
 Requires      Optionnel. Fichiers requis pour le bon fonctionnement de l'action. Voir ci-dessous.
 Default       Optionnel. Définit si l'action sera l'action par défaut (valeur par défaut: false).
 
======================================
 Ajouter des fichiers complémentaires
======================================

En utilisant la fonction "Requires", FileActionsManager va rechercher les fichiers requis dans
le même dossier que votre fichier ".seinf". Par exemple, l'architecture suivante est valide :

OptimiserImages
 |- OptimiserImages.seinf
 |- FreeImage.dll
 `- Riot.exe

En ouvrant"OptimiserImages.seinf", FileActionsManager va copier "FreeImage.dll"
et "Riot.exe" dans "%appdata%\ShellExtensions". De plus, le champ "Command" sera
adapté pour utiliser le chemin complet vers "Riot.exe" avant d'être écrit dans le registre.

Il est possible de partager un même exécutable entre deux actions, par exemple il est possible
de fournir "ffmpeg.exe" à la fois avec "ConvertirMP3.seinf" et "ConvertirAAC.seinf".
"ffmpeg.exe" sera placé dans "%appdata%\ShellExtensions", en tenant compte des actions
dépendant de "ffmpeg.exe". L'exécutable ne sera supprimé que lorsque la dernière action
qui en dépend est supprimée.

==================================
 Utilisation en ligne de commande
==================================

Comme alternative aux fichiers de configuration, FileActionsConsole peut être utilisé :

FileActionsConsole.exe add <extension(s)> <nom interne> <texte à afficher> <commande> [defaut=false]
FileActionsConsole.exe del <extension(s)> <nom interne>

Exemple avec l'action "Optimiser la taille" :

FileActionsConsole.exe add bmp,png,gif,jpg optimizeimagesize "Optimiser la taille" "Riot.exe "%1""
FileActionsConsole.exe del bmp,png,gif,jpg optimizeimagesize

Cet outil ne gère pas les fichiers complémentaires, il faudra donc les installer autrement.
Si vous n'en avez pas besoin, vous pouvez supprimer FileActionsConsole.exe sans souci.

=====
 FAQ
=====

Q: Lors de la création de certaines actions SANS le champ "Default=true",
   l'action devient quand même l'action par défaut !
R: Certains types de fichiers dans Windows n'ont pas d'action par défaut, et Windows prend la
   première action de la liste. Il est possible de corriger cela en définissant manuellement
   une action par défaut via le logiciel FileTypesMan de Nirsoft, ou bien en choisissant un
   nom interne du type zzmonaction avant création de sorte que celle-ci ne soit pas en
   première position dans la liste et donc ne soit pas choisie comme action par défaut.

Q: Que se passe-t-il si deux actions différentes sont fournies avec deux fichiers
   de même nom, par exemple des exécutables, mais dont le contenu est différent ?
R: Cela n'est pas géré et un seul des deux fichiers sera copié dans AppData.

Q: Comment désinstaller une action ?
R: Ouvrez de nouveau le fichier ".seinf" avec FileActionsManager.

Q: Comment modifier une action ?
R: Désinstallez-la, puis réinstallez-la.
   Egalement, désinstallez-la *avant* de modifier son nom interne.

Q: Où puis-je obtenir le code source, contribuer et obtenir de l'aide ?
R: Si possible en anglais ici : https://github.com/ORelio/FileActionsManager/
   Sinon, en français sur mon site internet : https://microzoom.fr/

=========
 Crédits
=========

Les icônes suivantes sont utilisées au sein de FileActionsManager :

 - Buuf icons par Mattahan: Text Editor icon
 - Buuf icons par Mattahan: Menu icon

Source: http://www.iconarchive.com/show/buuf-icons-by-mattahan/
Ces icônes sont licensiées sous Creative Commons Attribution-Noncommercial-Share Alike 4.0.

+--------------------+
| © 2015-2018 ORelio |
+--------------------+