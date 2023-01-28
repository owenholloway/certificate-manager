CREATE TABLE public.root_certificate_authorities (
    id serial4 NOT NULL,
    private_key bytea NOT NULL,
    public_key bytea NOT NULL,
    valid_from time with time zone NOT NULL,
    valid_till time with time zone NOT NULL
);
ALTER TABLE public.root_certificate_authorities ADD CONSTRAINT root_certificate_authorities_pk PRIMARY KEY (id);