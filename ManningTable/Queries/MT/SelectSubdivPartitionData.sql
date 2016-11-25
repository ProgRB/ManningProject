declare
begin
	open :c1 for select * from apstaff.subdiv_partition;
	open :c2 for select * from apstaff.subdiv_part_type;
end;
