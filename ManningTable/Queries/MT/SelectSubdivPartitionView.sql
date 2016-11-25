declare
begin
	open :c1 for select * from {0}.SUBDIV_ROLES_ALL 
			join (select subdiv_id, SUB_ACTUAL_SIGN, SUB_DATE_START, SUB_DATE_END, TYPE_SUBDIV_ID from apstaff.subdiv) using (subdiv_id)
		where APP_NAME='MANNING_TABLE' and sub_level=2
			and type_subdiv_id not in (6);
	open :c2 for select * from {0}.SUBDIV_PARTITION;
end;