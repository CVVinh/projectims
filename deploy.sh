#!/bin/bash
docker pull $CI_REGISTRY/$IMAGE_NAME_BACKEND:$IMAGE_TAG
docker pull $CI_REGISTRY/$IMAGE_NAME_FRONTEND:$IMAGE_TAG
docker rm -f $CONTAINER_NAME_BACKEND
docker rm -f $CONTAINER_NAME_FRONTEND
docker run --name=$CONTAINER_NAME_BACKEND -dp 8081:80 $CI_REGISTRY/$IMAGE_NAME_BACKEND:$IMAGE_TAG
docker run --name=$CONTAINER_NAME_FRONTEND -dp 80:80 $CI_REGISTRY/$IMAGE_NAME_FRONTEND:$IMAGE_TAG
docker rmi $(docker images --filter "dangling=true" -q --no-trunc)