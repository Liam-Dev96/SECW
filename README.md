# SECW
which documentation tool are we moving forward with? I think it may be best to use Docfx due to its auto gernerative html documents from xml comments instead of the alternatives such as sandcastle (which is a tad bit too old) or doxygen (compatible with diffrent langauges).

#I already installed Docfx on my device using "dotnet tool install -g docfx"
#and we can simply comment the desired descriptive sections using // and invoke its use with docfx build which as the document says should create the html file automatically in the _site folder. as for presentation am not too sure if it adds any styling or css but a simple document should suffice.

#Used command "dotnet list package --include-transitive > packages.txt" #(to update packages.txt run this command once more.)
in order to create a file similar to Requiremnts, to store all packages and Dependancies needed to be installed for this specific application to run.
the database helper is coded in C# and it includes a prototype of the required tables along side multiple comments to explain what each section does.


Validation libraries, login and sign up related related information
for email validation I plan to use the built in System.ComponentModel.DataAnnotations.
for password validation I plan to use Bcrypt.
will also need to update the dependencies or packages.txt folder after successfuly installing bcrypt...
Dependancies have been updated.

#Due to my assigned tasks including the creation of the db and login I also took it upon myself to create the other tables and columns required for the completion of the application


to ensure that the application runs, 
you firstly need to download the dependencies for sqllite.

list of commands 
dotnet clean
Functionality: Cleans the project by removing all compiled files (e.g., bin and obj folders).
Use Case: To ensure a fresh build by removing old or cached files.

I also deleted the bin and obj file from the previous demo project the click counter.

 dotnet workload update
Functionality: Updates all installed .NET workloads to their latest versions.
Use Case: To ensure that all workloads (e.g., Android, iOS, etc.) are up to date.

 dotnet add package Microsoft.Data.Sqlite
Functionality: Adds the Microsoft.Data.Sqlite NuGet package to the project.
Use Case: To use SQLite in a cross-platform manner, especially for .NET MAUI projects.

dotnet workload install android
Functionality: Installs the Android workload for .NET MAUI development.
Use Case: To enable building and running the project on Android devices.

 dotnet list package --include-transitive > packages.txt
Functionality: Lists all the NuGet packages (including transitive dependencies) used in the project and saves the output to a file named packages.txt.
Use Case: To document all dependencies required for the project.

 docfx build
Functionality: Builds the documentation using Docfx and generates the output in the _site folder.
Use Case: To create HTML documentation for your project based on XML comments.
just testing to see if the auto document features work as expected or not.