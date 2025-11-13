// Check authentication
const token = localStorage.getItem('token');
const userType = localStorage.getItem('userType');

if (!token || userType !== 'admin') {
    window.location.href = 'login.html';
}

// Set admin name
document.getElementById('adminName').textContent = `Welcome, ${localStorage.getItem('userName')}`;

let enrolledStudentsData = [];

// API base URL
const API_BASE = '/api';

// Show section
function showSection(sectionName) {
    const sections = document.querySelectorAll('.section');
    sections.forEach(s => s.classList.remove('active'));
    document.getElementById(sectionName).classList.add('active');

    const buttons = document.querySelectorAll('.sidebar-menu button');
    buttons.forEach(b => b.classList.remove('active'));
    event.target.classList.add('active');

    // Load data for the section
    if (sectionName === 'students') loadStudents();
    if (sectionName === 'teachers') loadTeachers();
    if (sectionName === 'courses') loadCourses();
}

// Logout
function logout() {
    localStorage.clear();
    window.location.href = 'login.html';
}

// ========== STUDENTS MANAGEMENT ==========

async function loadStudents() {
    try {
        const response = await fetch(`${API_BASE}/admin/students`, {
            headers: { 'Authorization': `Bearer ${token}` }
        });
        const students = await response.json();

        const tbody = document.getElementById('studentTableBody');
        tbody.innerHTML = students.map(s => `
            <tr>
                <td>${s.studentId}</td>
                <td>${s.firstName} ${s.lastName}</td>
                <td>${s.email}</td>
                <td>${s.username}</td>
                <td>
                    <button class="btn-action btn-edit" onclick="editStudent(${s.studentId})">Edit</button>
                    <button class="btn-action btn-delete" onclick="deleteStudent(${s.studentId})">Delete</button>
                </td>
            </tr>
        `).join('');
    } catch (error) {
        console.error('Error loading students:', error);
    }
}

function openAddStudentModal() {
    document.getElementById('studentModalTitle').textContent = 'Add Student';
    document.getElementById('studentForm').reset();
    document.getElementById('studentId').value = '';
    document.getElementById('studentPassword').required = true;
    document.getElementById('studentModal').classList.add('show');
}

async function editStudent(id) {
    try {
        const response = await fetch(`${API_BASE}/admin/students/${id}`, {
            headers: { 'Authorization': `Bearer ${token}` }
        });
        const student = await response.json();

        document.getElementById('studentModalTitle').textContent = 'Edit Student';
        document.getElementById('studentId').value = student.studentId;
        document.getElementById('studentFirstName').value = student.firstName;
        document.getElementById('studentLastName').value = student.lastName;
        document.getElementById('studentEmail').value = student.email;
        document.getElementById('studentUsername').value = student.username;
        document.getElementById('studentPassword').required = false;
        document.getElementById('studentModal').classList.add('show');
    } catch (error) {
        console.error('Error loading student:', error);
    }
}

function closeStudentModal() {
    document.getElementById('studentModal').classList.remove('show');
}

document.getElementById('studentForm').addEventListener('submit', async function (e) {
    e.preventDefault();

    const studentId = document.getElementById('studentId').value;
    const data = {
        firstName: document.getElementById('studentFirstName').value,
        lastName: document.getElementById('studentLastName').value,
        email: document.getElementById('studentEmail').value,
        username: document.getElementById('studentUsername').value,
        password: document.getElementById('studentPassword').value
    };

    try {
        const url = studentId ? `${API_BASE}/admin/students/${studentId}` : `${API_BASE}/admin/students`;
        const method = studentId ? 'PUT' : 'POST';

        const response = await fetch(url, {
            method: method,
            headers: {
                'Content-Type': 'application/json',
                'Authorization': `Bearer ${token}`
            },
            body: JSON.stringify(data)
        });

        if (response.ok) {
            closeStudentModal();
            loadStudents();
            alert(studentId ? 'Student updated successfully' : 'Student added successfully');
        } else {
            alert('Error saving student');
        }
    } catch (error) {
        console.error('Error saving student:', error);
        alert('Error saving student');
    }
});

async function deleteStudent(id) {
    if (!confirm('Are you sure you want to delete this student?')) return;

    try {
        const response = await fetch(`${API_BASE}/admin/students/${id}`, {
            method: 'DELETE',
            headers: { 'Authorization': `Bearer ${token}` }
        });

        if (response.ok) {
            loadStudents();
            alert('Student deleted successfully');
        } else {
            alert('Error deleting student');
        }
    } catch (error) {
        console.error('Error deleting student:', error);
        alert('Error deleting student');
    }
}

// ========== TEACHERS MANAGEMENT ==========

async function loadTeachers() {
    try {
        const response = await fetch(`${API_BASE}/admin/teachers`, {
            headers: { 'Authorization': `Bearer ${token}` }
        });
        const teachers = await response.json();

        const tbody = document.getElementById('teacherTableBody');
        tbody.innerHTML = teachers.map(t => `
            <tr>
                <td>${t.teacherId}</td>
                <td>${t.firstName} ${t.lastName}</td>
                <td>${t.email}</td>
                <td>${t.username}</td>
                <td>
                    <button class="btn-action btn-edit" onclick="editTeacher(${t.teacherId})">Edit</button>
                    <button class="btn-action btn-delete" onclick="deleteTeacher(${t.teacherId})">Delete</button>
                </td>
            </tr>
        `).join('');
    } catch (error) {
        console.error('Error loading teachers:', error);
    }
}

function openAddTeacherModal() {
    document.getElementById('teacherModalTitle').textContent = 'Add Teacher';
    document.getElementById('teacherForm').reset();
    document.getElementById('teacherId').value = '';
    document.getElementById('teacherPassword').required = true;
    document.getElementById('teacherModal').classList.add('show');
}

async function editTeacher(id) {
    try {
        const response = await fetch(`${API_BASE}/admin/teachers/${id}`, {
            headers: { 'Authorization': `Bearer ${token}` }
        });
        const teacher = await response.json();

        document.getElementById('teacherModalTitle').textContent = 'Edit Teacher';
        document.getElementById('teacherId').value = teacher.teacherId;
        document.getElementById('teacherFirstName').value = teacher.firstName;
        document.getElementById('teacherLastName').value = teacher.lastName;
        document.getElementById('teacherEmail').value = teacher.email;
        document.getElementById('teacherUsername').value = teacher.username;
        document.getElementById('teacherPassword').required = false;
        document.getElementById('teacherModal').classList.add('show');
    } catch (error) {
        console.error('Error loading teacher:', error);
    }
}

function closeTeacherModal() {
    document.getElementById('teacherModal').classList.remove('show');
}

document.getElementById('teacherForm').addEventListener('submit', async function (e) {
    e.preventDefault();

    const teacherId = document.getElementById('teacherId').value;
    const data = {
        firstName: document.getElementById('teacherFirstName').value,
        lastName: document.getElementById('teacherLastName').value,
        email: document.getElementById('teacherEmail').value,
        username: document.getElementById('teacherUsername').value,
        password: document.getElementById('teacherPassword').value
    };

    try {
        const url = teacherId ? `${API_BASE}/admin/teachers/${teacherId}` : `${API_BASE}/admin/teachers`;
        const method = teacherId ? 'PUT' : 'POST';

        const response = await fetch(url, {
            method: method,
            headers: {
                'Content-Type': 'application/json',
                'Authorization': `Bearer ${token}`
            },
            body: JSON.stringify(data)
        });

        if (response.ok) {
            closeTeacherModal();
            loadTeachers();
            alert(teacherId ? 'Teacher updated successfully' : 'Teacher added successfully');
        } else {
            alert('Error saving teacher');
        }
    } catch (error) {
        console.error('Error saving teacher:', error);
        alert('Error saving teacher');
    }
});

async function deleteTeacher(id) {
    if (!confirm('Are you sure you want to delete this teacher?')) return;

    try {
        const response = await fetch(`${API_BASE}/admin/teachers/${id}`, {
            method: 'DELETE',
            headers: { 'Authorization': `Bearer ${token}` }
        });

        if (response.ok) {
            loadTeachers();
            alert('Teacher deleted successfully');
        } else {
            alert('Error deleting teacher');
        }
    } catch (error) {
        console.error('Error deleting teacher:', error);
        alert('Error deleting teacher');
    }
}

// ========== COURSES MANAGEMENT ==========

async function loadCourses() {
    try {
        const response = await fetch(`${API_BASE}/admin/courses`, {
            headers: { 'Authorization': `Bearer ${token}` }
        });
        const courses = await response.json();

        const tbody = document.getElementById('courseTableBody');
        tbody.innerHTML = courses.map(c => `
            <tr>
                <td>${c.courseCode}</td>
                <td>${c.courseName}</td>
                <td>${c.teacherName || 'Not Assigned'}</td>
                <td>${c.studentsEnrolled || 0}</td>
                <td>
                    <button class="btn-action btn-edit" onclick="editCourse(${c.courseId})">Edit</button>
                    <button class="btn-action btn-delete" onclick="deleteCourse(${c.courseId})">Delete</button>
                </td>
            </tr>
        `).join('');
    } catch (error) {
        console.error('Error loading courses:', error);
    }
}

async function openAddCourseModal() {
    document.getElementById('courseModalTitle').textContent = 'Add Course';
    document.getElementById('courseForm').reset();
    document.getElementById('courseId').value = '';
    enrolledStudentsData = [];
    document.getElementById('enrolledStudents').innerHTML = '';

    await loadTeachersDropdown();
    await loadStudentsDropdown();

    document.getElementById('courseModal').classList.add('show');
}

async function editCourse(id) {
    try {
        const response = await fetch(`${API_BASE}/admin/courses/${id}`, {
            headers: { 'Authorization': `Bearer ${token}` }
        });
        const course = await response.json();

        document.getElementById('courseModalTitle').textContent = 'Edit Course';
        document.getElementById('courseId').value = course.courseId;
        document.getElementById('courseCode').value = course.courseCode;
        document.getElementById('courseName').value = course.courseName;
        document.getElementById('courseDescription').value = course.description;
        document.getElementById('courseTeacher').value = course.teacherId || '';

        enrolledStudentsData = course.enrolledStudents || [];
        renderEnrolledStudents();

        await loadTeachersDropdown();
        await loadStudentsDropdown();

        document.getElementById('courseModal').classList.add('show');
    } catch (error) {
        console.error('Error loading course:', error);
    }
}

function closeCourseModal() {
    document.getElementById('courseModal').classList.remove('show');
}

async function loadTeachersDropdown() {
    try {
        const response = await fetch(`${API_BASE}/admin/teachers`, {
            headers: { 'Authorization': `Bearer ${token}` }
        });
        const teachers = await response.json();

        const select = document.getElementById('courseTeacher');
        select.innerHTML = '<option value="">Select Teacher</option>' +
            teachers.map(t => `<option value="${t.teacherId}">${t.firstName} ${t.lastName}</option>`).join('');
    } catch (error) {
        console.error('Error loading teachers dropdown:', error);
    }
}

async function loadStudentsDropdown() {
    try {
        const response = await fetch(`${API_BASE}/admin/students`, {
            headers: { 'Authorization': `Bearer ${token}` }
        });
        const students = await response.json();

        const select = document.getElementById('studentSelect');
        select.innerHTML = '<option value="">Select Student to Add</option>' +
            students.map(s => `<option value="${s.studentId}">${s.firstName} ${s.lastName}</option>`).join('');
    } catch (error) {
        console.error('Error loading students dropdown:', error);
    }
}

function addStudentToCourse() {
    const select = document.getElementById('studentSelect');
    const studentId = select.value;
    const studentName = select.options[select.selectedIndex].text;

    if (!studentId) {
        alert('Please select a student');
        return;
    }

    if (enrolledStudentsData.find(s => s.studentId == studentId)) {
        alert('Student already enrolled');
        return;
    }

    enrolledStudentsData.push({ studentId: studentId, studentName: studentName });
    renderEnrolledStudents();
    select.value = '';
}

function removeStudentFromCourse(studentId) {
    enrolledStudentsData = enrolledStudentsData.filter(s => s.studentId != studentId);
    renderEnrolledStudents();
}

function renderEnrolledStudents() {
    const container = document.getElementById('enrolledStudents');
    container.innerHTML = enrolledStudentsData.map(s => `
        <div class="student-item">
            <span>${s.studentName}</span>
            <button type="button" class="btn-remove" onclick="removeStudentFromCourse(${s.studentId})">Remove</button>
        </div>
    `).join('');
}

document.getElementById('courseForm').addEventListener('submit', async function (e) {
    e.preventDefault();

    const courseId = document.getElementById('courseId').value;
    const data = {
        courseCode: document.getElementById('courseCode').value,
        courseName: document.getElementById('courseName').value,
        description: document.getElementById('courseDescription').value,
        teacherId: document.getElementById('courseTeacher').value || null,
        studentIds: enrolledStudentsData.map(s => s.studentId)
    };

    try {
        const url = courseId ? `${API_BASE}/admin/courses/${courseId}` : `${API_BASE}/admin/courses`;
        const method = courseId ? 'PUT' : 'POST';

        const response = await fetch(url, {
            method: method,
            headers: {
                'Content-Type': 'application/json',
                'Authorization': `Bearer ${token}`
            },
            body: JSON.stringify(data)
        });

        if (response.ok) {
            closeCourseModal();
            loadCourses();
            alert(courseId ? 'Course updated successfully' : 'Course added successfully');
        } else {
            alert('Error saving course');
        }
    } catch (error) {
        console.error('Error saving course:', error);
        alert('Error saving course');
    }
});

async function deleteCourse(id) {
    if (!confirm('Are you sure you want to delete this course?')) return;

    try {
        const response = await fetch(`${API_BASE}/admin/courses/${id}`, {
            method: 'DELETE',
            headers: { 'Authorization': `Bearer ${token}` }
        });

        if (response.ok) {
            loadCourses();
            alert('Course deleted successfully');
        } else {
            alert('Error deleting course');
        }
    } catch (error) {
        console.error('Error deleting course:', error);
        alert('Error deleting course');
    }
}

// Load initial data
loadStudents();