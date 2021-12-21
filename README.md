# MobileBandSync

UWP mobile app to sync, view and export GPS based workouts and sleep tracked with the Microsoft Band

This app lets you continue tracking workouts with the device and is available for Windows 10 Mobile
and Windows Desktop (tested on Windows 10 and 11)
No smartphone is necessary to record workouts as the app is able to download and analyze the sensor 
log created by the Microsoft Band. At the moment, only GPS based workouts such as biking, running 
and walking can be detected. To track a walking workout, simply use the running tile as the hiking tile 
seems to have major GPS problems, probably because the power saving GPS is used.
The Sleep tracking overview shows the summary and HR data. Workout data is stored locally in a
SQLite database.

It is possible to export the workout database as well as export each
workout to Garmin Training Center file format (TCX) files.

I hope that I will find the time to add some more features like filtering the list of workouts.
