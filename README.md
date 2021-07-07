# MSSQL-to-MongoDB

A C# console app to convert a specific relational MS SQL database to MongoDB

## Requirements

- SQL Server

Restoring SQL database (see ./Database/What2Watch.7zip for the .bak-file).

- SQL Server Management Studio

(Optional) GUI for managing databases in SQL Server.

- MongoDB

Target database for after converting from MS SQL.

- MongoDBCompass

(Optional) GUI for managing databases in MongoDB.

- Visual Studio 2019

IDE for building C# projects and running this particular one.

## Setup

After installing all required programs, start MSSQL-to-MongoDB and use the 2 and 3 keys in the Console TUI to configure your MongoDB and SQL connection strings (examples given in menu). This information will be saved locally so this step won't have to be repeated each time when running the program. Use the '4'-key in the Console TUI to check if the entered information has been saved correctly.

## Usage

When properly set up and after pressing '1' in the Console TUI, the program will try to load the most important parts of the MS SQL database into a MongoDB one. Depending on your device, this will take about 5 to 10 minutes, after which all data should be loaded into MongoDB correctly.

---

### Disclaimer

This tool was only made for this specific database for a particular university project. The creator has no prior knowledge of MongoDB or NoSQL as a whole which is exactly what this study aims to improve. Skilled MongoDB or C# developers might look at this code and will get a migraine from the contents of these files for which I do not supply proper medicine, although the aim of this project was achieved since the conversion does succeed.
