create table public.issued_certificates (
    id serial4 not null,
    intermediate_ca_id int4 not null,
    certificate_name varchar null,
    private_key bytea not null,
    public_key bytea not null,
    certificate_data bytea not null,
    serial_no bytea not null,
    valid_from timestamp with time zone not null,
    valid_till timestamp with time zone not null
);

alter table public.issued_certificates 
    add constraint issued_certificates_pk primary key (id);

alter table public.issued_certificates 
    add constraint issued_certificates_intermediate_ca_id_fk 
        foreign key (intermediate_ca_id) references public.intermediate_certificate_authorities(id);

