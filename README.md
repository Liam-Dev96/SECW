for admin data back up, you can simplify it by creating a log of all admin actions kinda like i was doing, but add a new table to the database called adminbackup, give said table these columns,
changeID autoincrement, changeDate date time now ,changemade varchar 500, then dont replace the logging but add on to it by making a variable and assigning said variable the logging data then make a function which has the connection string and adds on to the db, then pass through the variable and save it doing so ensures that youve made a back up of all admin actions and attempts.

try
CREATE TABLE adminbackup (
    changeID INTEGER PRIMARY KEY AUTOINCREMENT,
    changeDate DATETIME DEFAULT CURRENT_TIMESTAMP,
    changemade VARCHAR(500)
);

catch
error()

![image](https://github.com/user-attachments/assets/e732581b-d0f1-435e-bc41-f6bd4ca6e705)
here is an example of what i mean
![image](https://github.com/user-attachments/assets/684c5043-c7d1-42b6-ac30-76ac3c686c4d)

we created the variable, passed through the error message.
we will next create the function to save into the database and continue passing through the messages periodicalling when they arrise
need to make anoher function that detects when these are used or called to then pass them through other wise everything will be passed through at once which will not be a useful log.

need to also create the new table with the fields stated above to log the data into.

![image](https://github.com/user-attachments/assets/16437a45-c514-4f9a-a5d4-d894dc893875)
another example with a different error message that passes in the users the error is related to
now the function to detect when the error shows up is where the genius comes in because there are a few methods of doing so and its up to you to choose which is best suited for you and the development of the project.


