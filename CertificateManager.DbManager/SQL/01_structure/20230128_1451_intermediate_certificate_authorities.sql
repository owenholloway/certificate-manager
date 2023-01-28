CREATE TABLE public.intermediate_certificate_authorities (
    id serial4 NOT NULL,
    root_ca_id int4 NOT NULL,
    certificate_name varchar NULL,
    private_key bytea NOT NULL,
    public_key bytea NOT NULL,
    certificate_data bytea NOT NULL,
    valid_from timestamp with time zone NOT NULL,
    valid_till timestamp with time zone NOT NULL
);

ALTER TABLE public.intermediate_certificate_authorities 
    ADD CONSTRAINT intermediate_certificate_authorities_pk PRIMARY KEY (id);

ALTER TABLE public.intermediate_certificate_authorities 
    ADD CONSTRAINT intermediate_certificate_authorities_root_ca_id_fk 
        FOREIGN KEY (root_ca_id) REFERENCES public.root_certificate_authorities(id);


