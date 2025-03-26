docker pull mcr.microsoft.com/presidio-analyzer
docker pull mcr.microsoft.com/presidio-anonymizer

# Run containers with default ports
docker run -d -p 5002:3000 mcr.microsoft.com/presidio-analyzer:latest

docker run -d -p 5001:3000 mcr.microsoft.com/presidio-anonymizer:latest

# Analyzer API Doc

https://microsoft.github.io/presidio/api-docs/api-docs.html