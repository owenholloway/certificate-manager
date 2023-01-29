create table public.root_requests (
	id serial4 not null,
	certificate_name varchar not null,
	keep_active bool not null default false
);

alter table public.root_requests 
    add constraint root_requests_pk primary key (id);

alter table public.root_certificate_authorities 
    add root_request_id int4 not null;

alter table public.root_certificate_authorities 
    add constraint root_certificate_authorities_root_request_id_fk 
        foreign key (root_request_id) references public.root_requests(id);
