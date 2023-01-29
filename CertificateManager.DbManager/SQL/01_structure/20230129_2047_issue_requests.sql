create table public.issue_requests (
	id serial4 not null,
    intermediate_ca_id int4 not null,
    certificate_signing_request varchar not null,
	keep_active bool not null default false
);

alter table public.issue_requests 
    add constraint issue_requests_pk primary key (id);

alter table public.issued_certificates 
    add issue_request_id int4 not null;

alter table public.issued_certificates 
    add constraint intermediate_certificate_authorities_intermediate_request_id_fk 
        foreign key (issue_request_id) references public.issue_requests(id);

alter table public.issue_requests
    add constraint issue_requests_intermediate_ca_id_fk
        foreign key (intermediate_ca_id) references public.intermediate_certificate_authorities(id);
