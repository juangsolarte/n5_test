import React, { useEffect, useState } from 'react';
import axios from 'axios';
import { Button, Container, List, ListItem, ListItemText, Typography } from '@mui/material';
import { Permission } from '../types/Permission';

const PermissionsList: React.FC = () => {
	const [permissions, setPermissions] = useState<Permission[]>([]);

	useEffect(() => {
		const fetchPermissions = async () => {
			try {
				const response = await axios.get<Permission[]>('http://localhost:5232/api/Permission/GetPermissions');
				setPermissions(response.data);
			} catch (error) {
				console.error('There was an error fetching the permissions!', error);
			}
		};

		fetchPermissions();
	}, []);

	const formatDate = (dateString: string) => {
		const options: Intl.DateTimeFormatOptions = { day: '2-digit', month: '2-digit', year: 'numeric' };
		return new Date(dateString).toLocaleDateString(undefined, options);
	};

	return (
		<Container>
			<h1>Permissions List</h1>
			<Button variant="contained" color="primary" href="/create">
				Create Permission
			</Button>
			<List>
				{permissions.map((permission) => (
					<ListItem key={permission.id}>
						<ListItemText
							primary={
							<Typography variant="h6" fontWeight="bold">
								{`${permission.employeeForename} ${permission.employeeSurname}`}
							</Typography>}
							secondary={
								<>
									<Typography component="span" variant="body2" color="textPrimary" fontWeight="bold">
										PermissionType:
									</Typography>{' '}
									<Typography component="span" variant="body2" color="textPrimary">
										{permission.permissionType.description}
									</Typography>
									<br />
									<Typography component="span" variant="body2" color="textPrimary" fontWeight="bold">
										Date:
									</Typography>{' '}
									<Typography component="span" variant="body2" color="textPrimary">
										{formatDate(permission.permissionDate)}
									</Typography>
									<br />
									<Button variant="outlined" color="secondary" href={`/update/${permission.id}`}>
										Modify
									</Button>
								</>
							}
						/>
					</ListItem>
				))}
			</List>
		</Container>
	);
};

export default PermissionsList;
