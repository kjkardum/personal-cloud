FROM postgres:17

# Update package lists
RUN apt-get update

# Install PostgreSQL extensions
RUN apt-get install -y --no-install-recommends \
    postgresql-contrib \
    postgresql-17-pgvector \
    postgresql-17-postgis

# Clean up
RUN rm -rf /var/lib/apt/lists/*

# Expose the PostgreSQL port
EXPOSE 5432
