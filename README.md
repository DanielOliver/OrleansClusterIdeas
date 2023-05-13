# OrleansClusterIdeas

## Getting started

1. Run docker-compose in `LocationAccess`
2. Open `http://127.0.0.1/swagger/index.html` for Swagger
3. Open redis-commander at `http://localhost:8081/` to see Orleans Silo cluster keys
4. Inspect and check that there are multiple instances of the .NET application running.
5. Using swagger or curl, create multiple locations. The value doesn't matter.
   ```
   curl -X 'POST' \
     'http://127.0.0.1/Locations' \
     -H 'accept: */*' \
     -H 'Content-Type: application/json' \
     -d '{
     "id": 0,
     "name": "string"
     }'
   ```
6. Attempt the getName endpoint for whatever Ids you want. The point is to watch the logs and see these requests get distributed across the silo.
   ```
   curl -X 'GET' \
     'http://127.0.0.1/Locations/{locationId}/name' \
     -H 'accept: */*'
   ```
7. Stop, restart, or scale multiple instances from docker compose.  Keep running the above requests for random ids and see them get distributed across hosts. 
8. Wait a couple minutes without changing anything and see Grains start getting deactivated.

## Tools

* JetBrains Rider
* .NET 7
* Microsoft Orleans
* Docker
