CREATE TABLE public.root_certificate_authorities (
    id serial4 NOT NULL,
    certificate_name varchar NULL,
    private_key bytea NOT NULL,
    public_key bytea NOT NULL,
    certificate_data bytea NOT NULL,
    valid_from timestamp with time zone NOT NULL,
    valid_till timestamp with time zone NOT NULL
);
ALTER TABLE public.root_certificate_authorities 
    ADD CONSTRAINT root_certificate_authorities_pk PRIMARY KEY (id);