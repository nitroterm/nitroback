# Nitroterm

WIP social network backend.

## Setup

Clone & build the project, then setup the database.

### Setup the database with Docker

+ Create a `mariadb` image with docker, then, start it.
+ Connect to the database with the following command :
    ```
    $ docker exec -it <container-name> mariadb --user root -p
    (enter your password)
    ```
+ Create the database in the SQL client :
    ```
    > create database nitroback;
    ```
+ Then, create the nitro user (replace 'xxxxxx' by your password) :
    > TODO: We shouldn't allow connection to the nitro account elsewhere than localhost
    ```
    > create user 'nitro'@'%' identified by 'xxxxxxxx'; 
    ```
+ Grant all privileges to the nitro user :
    ```
    > grant all privileges on *.* to 'nitro'@'%' identified by 'xxxxxxxx' with grant option;
    ```
+ Flush privileges, then exit :
    ```
    > flush privileges;
    > exit
    ```
+ Then, run the database's migrations :
    ```
    $ cd Nitroterm.Backend
    $ dotnet ef database update
    ```

Done !