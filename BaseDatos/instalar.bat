@echo off
sqllocaldb start MSSQLLocalDB
sqlcmd -S "(localdb)\MSSQLLocalDB" -i "%~dp0AcademiaIdiomas.sql"
pause
