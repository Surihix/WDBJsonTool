# WDBJsonTool

This program allows you to convert the WDB database files from the FF13 game trilogy to JSON file as well as allow you to convert the JSON file, back to WDB. the program should be launched from a command prompt terminal with a few argument switches to perform the conversion functions. a list of valid argument switches are given below:

**Game Codes:**
<br>``-ff131`` For FF13-1 WDB files
<br>``-ff132`` For FF13-2 and FF13-LR 's WDB files


<br>**Tool actions:**
<br>``-x`` Converts the WDB file's data into a new JSON file
<br>``-xi`` Converts the WDB file's data into a new JSON file without the fieldnames (only when gamecode is -ff131)
<br>``-c`` Converts the data in the JSON file into a new WDB file
<br>``-?`` Display the help page. will also display few argument examples too.

<br>Commandline usage examples:
<br>``WDBJsonTool.exe -?``
<br>``WDBJsonTool.exe -ff131 -x "auto_clip.wdb" ``
<br>``WDBJsonTool.exe -ff131 -xi "auto_clip.wdb" ``
<br>``WDBJsonTool.exe -ff131 -c "auto_clip.json" ``

## Important notes
- The program requires .net 6.0 Desktop Runtime to be installed on your PC. you can get it from this [page](https://dotnet.microsoft.com/en-us/download/dotnet/6.0).

- The WDB file or JSON file has to be specified after the game code and the tool action argument switches.

- Some of the JSON files will be quite large and its recommended to use a text editor that doesn't hang or freeze when viewing/editing such large JSON files. personally, I use VS code to edit these JSON files and would recommend using it if you are unsure as to which text editor to use.

- Field names will be present in the JSON file only for some of 13-1's WDB files. refer to this [page](https://github.com/LR-Research-Team/Datalog/wiki/WDB-Field-Names) for information about the field names.

- If you are adding new records in the WDB file, make sure to first increase the `recordCount` property's value present at the start of the JSON file. after that, add your new records according to the alphabetical order which can require adding them in between two existing records or after the last existing record.

- When editing numerical fields, make sure that your number does not exceed the bit amount value given in the field name.

- Please report any problems that you encounter with the converted JSON/WDB files by opening an issue page detailing the issue here or in the "Fabula Nova Crystallis: Modding Community" discord server.

## For developers
- Refer to this [page](https://github.com/LR-Research-Team/Datalog/wiki/WDB) for information about the file structure of the WDB file.
