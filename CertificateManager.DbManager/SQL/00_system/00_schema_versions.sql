create table schema_versions(
    name varchar(128) not null,
    executed timestamp with time zone default current_timestamp
)