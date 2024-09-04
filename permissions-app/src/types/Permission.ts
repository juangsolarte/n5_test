import { PermissionType } from './PermissionType';

export interface Permission {
	id?: number;
	employeeForename: string;
	employeeSurname: string;
	permissionType: PermissionType;
	permissionDate: string;
}