# Docker Quick Start Guide

This guide provides quick commands to get the Million Real Estate API running with Docker.

## 🚀 Quick Start

### Start Everything
```bash
docker-compose up -d
```

**Access the API:**
- API Base URL: http://localhost:8000
- OpenAPI Spec: http://localhost:8000/openapi/v1.json
- Swagger UI: http://localhost:8000/swagger
- MongoDB: localhost:27017

### Test the API
```bash
# Get all properties
curl http://localhost:8000/api/properties

# Get properties with filters
curl "http://localhost:8000/api/properties?name=Casa&minPrice=100000&maxPrice=500000"
```

## 📋 Common Commands

### View Logs
```bash
# All services
docker-compose logs -f

# API only
docker-compose logs -f api

# MongoDB only
docker-compose logs -f mongodb
```

### Stop Services
```bash
# Stop all services (keeps data)
docker-compose down

# Stop and remove all data
docker-compose down -v
```

### Rebuild
```bash
# Rebuild and restart API
docker-compose up -d --build api

# Rebuild everything
docker-compose up -d --build
```

### Check Status
```bash
docker-compose ps
```

## 🔧 Troubleshooting

### API won't start
```bash
# Check logs
docker-compose logs api

# Rebuild the image
docker-compose up -d --build api
```

### Port already in use
Edit `compose.yaml` and change the port mapping:
```yaml
ports:
  - "8000:8080"  # Change 8000 to another port like 9000
```

### Database connection issues
```bash
# Restart MongoDB
docker-compose restart mongodb

# Check MongoDB health
docker-compose ps mongodb
```

### Reset everything
```bash
# Stop and remove everything
docker-compose down -v

# Remove old images
docker rmi million-api

# Start fresh
docker-compose up -d --build
```

## 🗄️ Database Access

### MongoDB Shell
```bash
# Access MongoDB shell
docker exec -it million_mongodb mongosh -u admin -p password

# Then in the shell:
use million
db.properties.find()
```

### MongoDB Connection String
```
mongodb://admin:password@localhost:27017/
```

## 📊 Container Management

### View running containers
```bash
docker ps
```

### Stop a specific container
```bash
docker stop million_api
docker stop million_mongodb
```

### Remove containers
```bash
docker rm million_api
docker rm million_mongodb
```

### View container resources
```bash
docker stats
```

## 🔄 Development Workflow

### Make code changes
1. Edit your code
2. Rebuild and restart:
```bash
docker-compose up -d --build api
```

### Run tests before deploying
```bash
dotnet test
```

### Check API health
```bash
curl http://localhost:8000/api/properties
```

## 📦 Production Deployment

### Build production image
```bash
docker build -t million-api:latest -f million.api/Dockerfile .
```

### Run in production mode
```bash
docker run -d \
  --name million_api \
  -p 8000:8080 \
  -e ASPNETCORE_ENVIRONMENT=Production \
  -e MongoDbSettings__ConnectionString="mongodb://admin:password@mongodb:27017/" \
  -e MongoDbSettings__DatabaseName="million" \
  million-api:latest
```

## 🌐 Network Configuration

The Docker Compose setup creates an isolated network `million-network`:
- Services communicate using service names (e.g., `mongodb`)
- Exposed ports are mapped to localhost
- Database is not directly accessible from outside unless port 27017 is exposed

## 💾 Data Persistence

Data is stored in a named volume `mongodb_data`:
- Persists across container restarts
- Located in Docker's volume directory
- Can be backed up using `docker volume` commands

### Backup database
```bash
docker exec million_mongodb mongodump \
  --username admin \
  --password password \
  --authenticationDatabase admin \
  --out /tmp/backup

docker cp million_mongodb:/tmp/backup ./backup
```

### Restore database
```bash
docker cp ./backup million_mongodb:/tmp/backup

docker exec million_mongodb mongorestore \
  --username admin \
  --password password \
  --authenticationDatabase admin \
  /tmp/backup
```
