create table public.intermediate_certificate_authorities (
    id serial4 not null,
    root_ca_id int4 not null,
    certificate_name varchar null,
    private_key bytea not null,
    public_key bytea not null,
    certificate_data bytea not null,
    serial_no bytea not null,
    valid_from timestamp with time zone not null,
    valid_till timestamp with time zone not null
);

alter table public.intermediate_certificate_authorities 
    add constraint intermediate_certificate_authorities_pk primary key (id);

alter table public.intermediate_certificate_authorities 
    add constraint intermediate_certificate_authorities_root_ca_id_fk 
        foreign key (root_ca_id) references public.root_certificate_authorities(id);

