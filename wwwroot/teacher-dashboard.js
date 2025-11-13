// ========== GLOBAL CONFIG & STATE ==========

const API_BASE = '/api';
let currentCourseId = null;
let currentQuizId = null;

// ========== AUTH & API HELPER ==========

/**
 * A wrapper for fetch that automatically adds the auth token
 * and handles auth errors (401, 403) by logging the user out.
 */
async function apiFetch(url, options = {}) {
    const token = localStorage.getItem('token');
    const userType = localStorage.getItem('userType');

    // Auth check on every API call
    if (!token || userType !== 'teacher') {
        console.error('Authentication check failed. Redirecting to login.');
        logout(); // Use our existing logout function
        throw new Error('Unauthorized'); // Stop this function's execution
    }

    // Prepare headers
    const headers = { ...options.headers, 'Authorization': `Bearer ${token}` };

    // Don't set Content-Type for FormData; the browser must do it
    if (options.body instanceof FormData) {
        delete headers['Content-Type'];
    }
    // Default to JSON for other requests with a body
    else if (options.body && !headers['Content-Type']) {
        headers['Content-Type'] = 'application/json';
    }

    try {
        const response = await fetch(url, { ...options, headers });

        // Handle expired token or permission issues
        if (response.status === 401 || response.status === 403) {
            console.error('Token expired or forbidden. Logging out.');
            logout();
            throw new Error('Unauthorized');
        }

        // Handle other server errors
        if (!response.ok) {
            const errorText = await response.text();
            throw new Error(`API Error (${response.status}): ${errorText}`);
        }

        // Handle 'No Content' responses (e.g., successful PUT/DELETE)
        if (response.status === 204) {
            return null;
        }

        return response.json(); // Return the parsed JSON

    } catch (error) {
        console.error('Fetch failed:', error.message);
        throw error; // Re-throw to be caught by the calling function
    }
}

function logout() {
    localStorage.clear();
    window.location.href = 'login.html';
}

// ========== COURSES ==========

async function loadCourses() {
    try {
        const courses = await apiFetch(`${API_BASE}/teacher/courses`);
        const grid = document.getElementById('coursesGrid');

        if (courses.length === 0) {
            grid.innerHTML = '<p style="color: #666;">No courses assigned yet.</p>';
            return;
        }

        grid.innerHTML = courses.map(c => `
            <div class="course-card" onclick="viewCourse(${c.courseId})">
                <h3>${c.courseName}</h3>
                <p><strong>${c.courseCode}</strong></p>
                <p>${c.description || 'No description'}</p>
                <p class="student-count">${c.studentsEnrolled || 0} Students Enrolled</p>
            </div>
        `).join('');
    } catch (error) {
        console.error('Error loading courses:', error);
        document.getElementById('coursesGrid').innerHTML = '<p style="color: red;">Could not load courses.</p>';
    }
}

function showCoursesList() {
    document.querySelectorAll('.section').forEach(s => s.classList.remove('active'));
    document.getElementById('coursesList').classList.add('active');
}

async function viewCourse(courseId) {
    currentCourseId = courseId;
    try {
        const course = await apiFetch(`${API_BASE}/teacher/courses/${courseId}`);

        document.getElementById('courseTitle').textContent = course.courseName;

        // Load students
        const studentsBody = document.getElementById('studentsTableBody');
        if (course.students && course.students.length > 0) {
            studentsBody.innerHTML = course.students.map(s => `
                <tr>
                    <td>${s.studentId}</td>
                    <td>${s.firstName} ${s.lastName}</td>
                    <td>${s.email}</td>
                </tr>
            `).join('');
        } else {
            studentsBody.innerHTML = '<tr><td colspan="3" style="text-align: center; color: #666;">No students enrolled.</td></tr>';
        }

        // Load quizzes
        await loadQuizzes(courseId);

        document.querySelectorAll('.section').forEach(s => s.classList.remove('active'));
        document.getElementById('courseDetails').classList.add('active');
    } catch (error) {
        console.error('Error loading course:', error);
    }
}

async function loadQuizzes(courseId) {
    try {
        const quizzes = await apiFetch(`${API_BASE}/teacher/courses/${courseId}/quizzes`);
        const container = document.getElementById('quizzesList');

        if (quizzes.length === 0) {
            container.innerHTML = '<p style="color: #666;">No quizzes/activities created yet.</p>';
            return;
        }

        container.innerHTML = quizzes.map(q => `
            <div class="quiz-item">
                <div class="quiz-info">
                    <h4>${q.title}</h4>
                    <p>${q.description || ''}</p>
                    <p><small>Due: ${new Date(q.dueDate).toLocaleString()} | Points: ${q.totalPoints}</small></p>
                </div>
                <div>
                    <span class="submissions-count">${q.submissions || 0} Submissions</span>
                    <button class="btn-action btn-view" onclick="viewSubmissions(${q.quizId})">View Submissions</button>
                </div>
            </div>
        `).join('');
    } catch (error) {
        console.error('Error loading quizzes:', error);
    }
}

// ========== QUIZ CREATION ==========

function openCreateQuizModal() {
    document.getElementById('quizForm').reset();
    document.getElementById('quizModal').classList.add('show');
}

function closeQuizModal() {
    document.getElementById('quizModal').classList.remove('show');
}

document.getElementById('quizForm').addEventListener('submit', async function (e) {
    e.preventDefault();

    const formData = new FormData();
    formData.append('courseId', currentCourseId);
    formData.append('title', document.getElementById('quizTitle').value);
    formData.append('description', document.getElementById('quizDescription').value);
    formData.append('dueDate', document.getElementById('quizDueDate').value);
    formData.append('totalPoints', document.getElementById('quizPoints').value);
    formData.append('file', document.getElementById('quizFile').files[0]);

    try {
        // apiFetch will handle FormData headers automatically
        await apiFetch(`${API_BASE}/teacher/quizzes`, {
            method: 'POST',
            body: formData
        });

        closeQuizModal();
        loadQuizzes(currentCourseId);
        alert('Quiz/Activity created successfully!');
    } catch (error) {
        console.error('Error creating quiz:', error);
        alert(`Error creating quiz: ${error.message}`);
    }
});

// ========== SUBMISSIONS ==========

async function viewSubmissions(quizId) {
    currentQuizId = quizId;

    // This is the correct structure for the try...catch block
    try {
        const data = await apiFetch(`${API_BASE}/teacher/quizzes/${quizId}/submissions`);

        // NOTE: You used 'quizTitle' ID here, which is also in the create quiz modal.
        // I recommend using a different ID, like 'quizSubmissionTitle'
        document.getElementById('quizSubmissionTitle').textContent = `Submissions for: ${data.quizTitle}`;

        const tbody = document.getElementById('submissionsTableBody');
        if (data.submissions.length === 0) {
            tbody.innerHTML = '<tr><td colspan="4" style="text-align: center; color: #666;">No submissions yet</td></tr>';
        } else {
            tbody.innerHTML = data.submissions.map(s => `
                <tr>
                    <td>${s.studentName}</td>
                    <td>${new Date(s.submittedDate).toLocaleString()}</td>
                    <td>${s.score !== null ? s.score + '/' + data.totalPoints : 'Not graded'}</td>
                    <td>
                        <button class="btn-action btn-view" onclick="viewSubmissionFile('${s.filePath}')">View PDF</button>
                        <button class="btn-action btn-score" onclick="openScoreModal(${s.submissionId}, '${s.studentName}', ${data.totalPoints}, ${s.score}, '${s.filePath}')">Score</button>
                    </td>
                </tr>
            `).join('');
        }

        // This part should run on success, outside the if/else
        document.querySelectorAll('.section').forEach(s => s.classList.remove('active'));
        document.getElementById('quizSubmissions').classList.add('active');

    } catch (error) {
        console.error('Error loading submissions:', error);
    }
}

function backToCourseDetails() {
    viewCourse(currentCourseId);
}

function viewSubmissionFile(filePath) {
    // We get the token here to append it as a query param if needed
    // for file downloads, as 'Authorization' headers can't be set
    // on a window.open() or <a> tag.
    // NOTE: This is less secure. A better way is an intermediate
    // endpoint that validates the token and serves the file.
    // But for a simple fix, this opens the path.
    window.open(`${API_BASE}/files/${filePath}`, '_blank');
}

// ========== SCORING ==========

function openScoreModal(submissionId, studentName, maxPoints, currentScore, filePath) {
    document.getElementById('submissionId').value = submissionId;
    document.getElementById('studentNameScore').textContent = studentName;
    document.getElementById('maxPoints').textContent = maxPoints;
    document.getElementById('scoreInput').max = maxPoints;
    document.getElementById('scoreInput').value = currentScore || '';
    document.getElementById('scoreFeedback').value = ''; // Clear old feedback
    document.getElementById('submissionFileLink').href = `${API_BASE}/files/${filePath}`;
    document.getElementById('scoreModal').classList.add('show');
}

function closeScoreModal() {
    document.getElementById('scoreModal').classList.remove('show');
}

document.getElementById('scoreForm').addEventListener('submit', async function (e) {
    e.preventDefault();

    const submissionId = document.getElementById('submissionId').value;
    const data = {
        score: parseFloat(document.getElementById('scoreInput').value),
        feedback: document.getElementById('scoreFeedback').value
    };

    // Simple validation
    if (isNaN(data.score)) {
        alert('Please enter a valid number for the score.');
        return;
    }

    try {
        await apiFetch(`${API_BASE}/teacher/submissions/${submissionId}/score`, {
            method: 'PUT',
            body: JSON.stringify(data)
        });

        closeScoreModal();
        viewSubmissions(currentQuizId);
        alert('Score saved successfully!');
    } catch (error) {
        console.error('Error saving score:', error);
        alert(`Error saving score: ${error.message}`);
    }
});

// ========== INITIALIZATION ==========

// Wait for the DOM to be fully loaded before running any script
document.addEventListener('DOMContentLoaded', () => {
    // Perform the initial auth check
    const token = localStorage.getItem('token');
    const userType = localStorage.getItem('userType');

    if (!token || userType !== 'teacher') {
        window.location.href = 'login.html';
        return; // Stop script execution
    }

    // Set welcome message
    document.getElementById('teacherName').textContent = `Welcome, ${localStorage.getItem('userName')}`;

    // Load initial data
    loadCourses();
});