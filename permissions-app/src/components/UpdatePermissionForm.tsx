import React, { useState, useEffect, ChangeEvent, FormEvent } from 'react';
import axios from 'axios';
import { TextField, Button, Container, Box, SelectChangeEvent, FormControl, InputLabel, Select, MenuItem } from '@mui/material';
import { useParams, useNavigate } from 'react-router-dom';
import { PermissionType } from '../types/PermissionType';
import { Permission } from '../types/Permission';

const UpdatePermissionForm: React.FC = () => {
	const { id } = useParams<{ id: string }>();
	const [formData, setFormData] = useState<Permission>(() => ({
		employeeForename: '',
		employeeSurname: '',
		permissionType: { id: 0, description: '' },
		permissionDate: '',
	}));

	const [permissionTypes, setPermissionTypes] = useState<PermissionType[]>([]);

	const navigate = useNavigate();

	useEffect(() => {
		const fetchPermission = async () => {
			try {
				const response = await axios.get(`http://localhost:5232/api/Permission/GetPermissionById/${id}`);
				const permissionData = response.data;
        const formattedDate = permissionData.permissionDate.split('T')[0];
        setFormData({ ...permissionData, permissionDate: formattedDate });
			} catch (error) {
				console.error('There was an error fetching the permission!', error);
			}
		};

		const fetchPermissionTypes = async () => {
      try {
        const response = await axios.get<PermissionType[]>('http://localhost:5232/api/PermissionType/GetPermissionTypes');
        setPermissionTypes(response.data);
      } catch (error) {
        console.error('There was an error fetching the permission types!', error);
      }
    };

		fetchPermission();
		fetchPermissionTypes();
	}, [id]);

	const handleChange = (e: ChangeEvent<HTMLInputElement>) => {
		setFormData({ ...formData, [e.target.name]: e.target.value });
	};

	const handleSelectChange = (e: SelectChangeEvent<number>) => {
	  setFormData({ ...formData, [e.target.name as string]: e.target.value });
	};

	const handleSubmit = async (e: FormEvent) => {
		e.preventDefault();
		try {
			const response = await axios.put(`http://localhost:5232/api/Permission/ModifyPermission/${id}`, formData);
			alert('Permission updated successfully!');
			console.log(response.data);
			navigate('/');
		} catch (error) {
			console.error('There was an error updating the permission!', error);
		}
	};

	return (
		<Container>
			<h1>Update Permission</h1>
			<form onSubmit={handleSubmit}>
				<TextField
					label="Employee Forename"
					name="employeeForename"
					value={formData.employeeForename}
					onChange={handleChange}
					fullWidth
					margin="normal"
				/>
				<TextField
					label="Employee Surname"
					name="employeeSurname"
					value={formData.employeeSurname}
					onChange={handleChange}
					fullWidth
					margin="normal"
				/>
				<FormControl fullWidth margin="normal">
          <InputLabel>Permission Type</InputLabel>
          <Select
            name="permissionTypeId"
            value={formData.permissionType.id}
            onChange={handleSelectChange}
          >
            {permissionTypes.map((type) => (
              <MenuItem key={type.id} value={type.id}>
                {type.description}
              </MenuItem>
            ))}
          </Select>
        </FormControl>
				<TextField
					label="Permission Date"
					name="permissionDate"
					type="date"
					value={formData.permissionDate}
					onChange={handleChange}
					fullWidth
					margin="normal"
					InputLabelProps={{
						shrink: true,
					}}
				/>
				<Box display="flex" gap={2}>
					<Button type="submit" variant="contained" color="primary">
						Update
					</Button>
					<Button type="button" variant="contained" onClick={() => navigate('/')} color="error">
						Cancel
					</Button>
				</Box>
			</form>
		</Container>
	);
};

export default UpdatePermissionForm;
