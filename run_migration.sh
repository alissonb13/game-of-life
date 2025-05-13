#!/bin/bash

echo "🕒 Wait for PostgreSQL to be ready..."

while ! nc -z db 5432; do
  sleep 0.5
done

echo "✅ PostgreSQL is ready!"

echo "🔄 Applying migrations..."
dotnet ef database update --project GameOfLife.Api --no-build

echo "🚀 Start the application..."
dotnet GameOfLife.Api.dll