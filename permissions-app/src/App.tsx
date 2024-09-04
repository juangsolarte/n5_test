import React from 'react';
import { BrowserRouter as Router, Route, Routes } from 'react-router-dom';
import CreatePermissionForm from './components/CreatePermissionForm';
import UpdatePermissionForm from './components/UpdatePermissionForm';
import PermissionsList from './components/PermissionsList';

const App: React.FC = () => {
  return (
    <Router>
      <Routes>
        <Route path="/create" element={<CreatePermissionForm />} />
        <Route path="/update/:id" element={<UpdatePermissionForm />} />
        <Route path="/" element={<PermissionsList />} />
      </Routes>
    </Router>
  );
};

export default App;
