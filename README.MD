docker-compose down
docker-compose up -d
Run

Clickhouse container:

clickhouse-client --query "CREATE TABLE new_projects_created_queue (Id UUID, Name String, timestamp UInt64) ENGINE = Kafka('broker:19092', 'NewProjectsCreated', 'consumer-group-2', 'JSONEachRow');"

clickhouse-client --query "CREATE TABLE daily (day Date, Name String, total UInt64) ENGINE = SummingMergeTree() ORDER BY (day);"

clickhouse-client --query "CREATE MATERIALIZED VIEW consumer TO daily AS SELECT toDate(toDateTime(timestamp)) AS day, count() as total FROM default.new_projects_created_queue GROUP BY day;"