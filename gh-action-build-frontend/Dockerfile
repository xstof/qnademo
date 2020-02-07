FROM node:12-alpine3.9
ARG sourcelocation
RUN npm install -g @quasar/cli
WORKDIR /app
COPY $sourcelocation .
RUN npm install
RUN npm rebuild node-sass
RUN quasar build
