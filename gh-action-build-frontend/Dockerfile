# develop stage
FROM node:13.7-alpine as develop-stage
ARG source-location
WORKDIR /app
COPY ${source-location} .
RUN yarn global add @quasar/cli
RUN yarn
RUN quasar build
