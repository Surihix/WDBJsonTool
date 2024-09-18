# WDBJsonTool

This program allows you to convert the WDB database files from the FF13 game trilogy to JSON file as well as allow you to convert the JSON file, back to WDB. the program should be launched from a command prompt terminal with a few argument switches to perform the conversion functions. a list of valid argument switches are given below:

**Game Codes:**
<br>``-ff131`` For FF13-1 WDB files
<br>``-ff132`` For FF13-2 and FF13-LR 's WDB files


<br>**Tool actions:**
<br>``-x`` Converts the WDB file's data into a new JSON file
<br>``-xi`` Converts the WDB file's data into a new JSON file without the fieldnames (only when gamecode is -ff131)
<br>``-c`` Converts the data in the JSON file into a new WDB file
<br>``-?`` or ``-h`` Display the help page. will also display few argument examples too.
<br>

## Important notes
- Some of the JSON files will be quite large and its recommended to use a text editor that doesn't hang up or freeze when viewing/editing such files. personally, I use VS code to edit these JSON files and would recommend using it, if you are unsure as to which text editor to use for editing these JSON files.

- If you are adding new records in the WDB file, make sure to first increase the `recordCount` property's value present at the start portion of the JSON file. after that, add your new records according to the alphabetical order.

- Please report any issues that you encounter with the converted JSON/WDB files here or in the Nova Chrysalia modding discord server.

## For developers
- Refer to this [page](https://github.com/LR-Research-Team/Datalog/wiki/WDB) for information about the structure of the WDB file.
