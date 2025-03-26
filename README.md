# BookHotel
# Run
+ open file "appsettings.json"
+ change DefaultConnection
+ Save file
+ create database name BookHotel
+ dotnet ef database update





# Dont read and follow
+ docker network create bookhotel-net
+ docker network connect bookhotel-net sqlserver-container
+ docker run -d --name bookhotel-api --network="bookhotel-net" -p 5000:5000 bookhotel-api

# Recharge Migration
docker run --rm \ --network="host" \ bookhotel-api \ dotnet ef database update

# Run
+ docker start (name-container)

# Stop
+ docker stop (name-container)

# DELETE
+ docker rm bookhotel-api sqlserver-container

# HOld info your db
+ git stash
+ git pull --force
+ git stash pop  # Lấy lại các thay đổi đã stash

