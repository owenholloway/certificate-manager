create table public.intermediate_requests (
	id serial4 not null,
    root_ca_id int4 not null,
	certificate_name varchar not null,
	keep_active bool not null default false
);

alter table public.intermediate_requests 
    add constraint intermediate_requests_pk primary key (id);

alter table public.intermediate_certificate_authorities 
    add intermediate_request_id int4 not null;

alter table public.intermediate_certificate_authorities 
    add constraint intermediate_certificate_authorities_intermediate_request_id_fk 
        foreign key (intermediate_request_id) references public.intermediate_requests(id);

alter table public.intermediate_requests
    add constraint intermediate_requests_root_ca_id_fk
        foreign key (root_ca_id) references public.root_certificate_authorities(id);
