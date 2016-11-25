declare
begin
	open :c1 for select * from apstaff.INDIVID_PROTECTION order by code_protection;
	open :c2 for select * from apstaff.TYPE_INDIVID_PROTECTION order by TYPE_PROTECTION_NAME;
end;
