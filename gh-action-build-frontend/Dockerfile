# develop stage
FROM node:13.7-alpine as develop-stage
ARG sourcelocation
WORKDIR /app
COPY $sourcelocation .
RUN yarn global add @quasar/cli
RUN yarn
RUN quasar build
