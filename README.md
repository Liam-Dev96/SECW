# SECW

## Documentation Tool
We are moving forward with **Docfx** for documentation due to its ability to auto-generate HTML documents from XML comments. This makes it a better choice compared to alternatives like Sandcastle (outdated) or Doxygen (multi-language support but less streamlined for our needs).

### Docfx Installation
Docfx is already installed using the following command:
```bash
dotnet tool install -g docfx
```

### Usage
1. Add descriptive sections in your code using `//` comments.
2. Run the following command to generate the documentation:
```bash
docfx build
```
This will create the HTML files in the `_site` folder.

3. To serve the generated documentation locally:
```bash
docfx serve _site/docs
```

---

## Dependencies and Packages
To document all dependencies, the following command was used:
```bash
dotnet list package --include-transitive > packages.txt
```
This creates a `packages.txt` file listing all required packages and dependencies for the application.

### Updates
- Dependencies have been updated after installing **Bcrypt** for password validation.
- SQLite dependencies need to be downloaded for the application to run.

---

## Commands and Their Functionality

### General Commands
- **`dotnet clean`**
- **Functionality**: Cleans the project by removing all compiled files (e.g., `bin` and `obj` folders).
- **Use Case**: Ensures a fresh build by removing old or cached files.

- **`dotnet workload update`**
- **Functionality**: Updates all installed .NET workloads to their latest versions.
- **Use Case**: Keeps workloads (e.g., Android, iOS) up to date.

- **`dotnet workload install android`**
- **Functionality**: Installs the Android workload for .NET MAUI development.
- **Use Case**: Enables building and running the project on Android devices.

### SQLite and Dependencies
- **`dotnet add package Microsoft.Data.Sqlite`**
- **Functionality**: Adds the `Microsoft.Data.Sqlite` NuGet package to the project.
- **Use Case**: Enables SQLite usage in a cross-platform manner.

- **`dotnet list package --include-transitive > packages.txt`**
- **Functionality**: Lists all NuGet packages (including transitive dependencies) and saves them to `packages.txt`.
- **Use Case**: Documents all dependencies required for the project.

### Testing and Mocking
- **`dotnet new xunit -n SECW.Tests`**
- **Functionality**: Creates a new xUnit test project named `SECW.Tests`.
- **Use Case**: Sets up a testing framework for the project.

- **`dotnet add package Moq`**
- **Functionality**: Adds the `Moq` NuGet package for mocking dependencies in unit tests.
- **Use Case**: Simplifies unit testing by mocking objects.

### Documentation
- **`docfx build`**
- **Functionality**: Builds the documentation using Docfx and generates the output in the `_site` folder.
- **Use Case**: Creates HTML documentation for the project based on XML comments.

- **`docfx serve _site/docs`**
- **Functionality**: Serves the generated documentation locally via a web server.
- **Use Case**: Allows previewing the documentation in a browser.

- **`dotnet add package xunit`**
- **Functionality**: Adds the `xunit` NuGet package to the project.
- **Use Case**: Enables unit testing using the xUnit framework.

---

## Notes
- The database helper is coded in C# and includes a prototype of the required tables with comments explaining each section.
- For password validation, we are using **Bcrypt**.

---

## important(ingore all warnings that pop up)
- **'dotnet test SECW.Tests.Solution.sln'**
this is how to run the unit tests which are implemented using docker in order to isolate the 2 csproj files, I kept running into errors with the dependencies being duplicated and when i deleted one and refrenced the other file it kept saying the dependencies were missing, then even after suppressing the errors the app kept crashing while the unit tests worked, and after I fixed the app the unit tests kept crashing, i created a new project which had the aim of sharing files and code between projects passing data back and forth to classes and even then i ran into dependency issues thats when i remembered that docker existed and that it can simplify all of this, heads up the project has about 400+ warnings, ignore them all or supress them either way it will run without an issue further testing is required though.

# testing added
I added one testing file with no code coverage
from what i expect this tests about at least 80% of operations and tasks going through in this application
reasoning being that it creates a database, mimics table creation based on fields similar to databasehelper, goes through crud operations, then deletes the temporary directory after.