#This wraps the azurite node process inside a Docker container on both ARM64 as x64
FROM node:lts-alpine

ENV NODE_ENV=production

RUN npm install -g azurite

VOLUME [ "/data" ]

# Blob Storage Port
EXPOSE 10000
# Queue Storage Port
EXPOSE 10001

CMD ["azurite", "-l", "/data", "--blobHost", "0.0.0.0","--queueHost", "0.0.0.0", "--skipApiVersionCheck", "--loose"]