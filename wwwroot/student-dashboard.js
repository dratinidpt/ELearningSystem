// Check authentication
const token = localStorage.getItem('token');
const userType = localStorage.getItem('userType');
const studentId = localStorage.getItem('userId');

if (!token || userType !== 'student') {
    window.location.href = 'login.html';
}

document.getElementById('studentName').textContent = `Welcome, ${localStorage.getItem('userName')}`;

const API_BASE = '/api';
let currentCourseId = null;

function logout() {
    localStorage.clear();
    window.location.href = 'login.html';
}

// ========== COURSES ==========

async function loadCourses() {
    try {
        const response = await fetch(`${API_BASE}/student/courses`, {
            headers: { 'Authorization': `Bearer ${token}` }
        });
        const courses = await response.json();

        const grid = document.getElementById('coursesGrid');
        if (courses.length === 0) {
            grid.innerHTML = '<p style="color: #666;">You are not enrolled in any courses yet.</p>';
            return;
        }

        grid.innerHTML = courses.map(c => `
            <div class="course-card" onclick="viewCourse(${c.courseId})">
                <h3>${c.courseName}</h3>
                <p><strong>${c.courseCode}</strong></p>
                <p>${c.description || 'No description'}</p>
                <p class="teacher-name">Teacher: ${c.teacherName}</p>
            </div>
        `).join('');
    } catch (error) {
        console.error('Error loading courses:', error);
    }
}

function showCoursesList() {
    document.querySelectorAll('.section').forEach(s => s.classList.remove('active'));
    document.getElementById('coursesList').classList.add('active');
}

async function viewCourse(courseId) {
    currentCourseId = courseId;

    try {
        const response = await fetch(`${API_BASE}/student/courses/${courseId}`, {
            headers: { 'Authorization': `Bearer ${token}` }
        });
        const course = await response.json();

        document.getElementById('courseTitle').textContent = course.courseName;
        document.getElementById('courseCode').textContent = course.courseCode;
        document.getElementById('courseTeacher').textContent = course.teacherName;
        document.getElementById('courseDescription').textContent = course.description || 'No description available';

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
        const response = await fetch(`${API_BASE}/student/courses/${courseId}/quizzes`, {
            headers: { 'Authorization': `Bearer ${token}` }
        });
        const quizzes = await response.json();

        const container = document.getElementById('quizzesList');
        if (quizzes.length === 0) {
            container.innerHTML = '<p style="color: #666;">No quizzes/activities available yet.</p>';
            return;
        }

        container.innerHTML = quizzes.map(q => {
            let status = 'pending';
            let statusText = 'Not Submitted';
            let statusClass = 'status-pending';

            if (q.submittedDate) {
                if (q.score !== null) {
                    status = 'graded';
                    statusText = 'Graded';
                    statusClass = 'status-graded';
                } else {
                    status = 'submitted';
                    statusText = 'Submitted';
                    statusClass = 'status-submitted';
                }
            }

            const dueDate = new Date(q.dueDate);
            const now = new Date();
            const isOverdue = dueDate < now && status === 'pending';

            return `
                <div class="quiz-card">
                    <div class="quiz-header">
                        <div>
                            <h3>${q.title}</h3>
                            <span class="quiz-status ${statusClass}">${statusText}</span>
                        </div>
                    </div>
                    <div class="quiz-info">
                        <p>${q.description || ''}</p>
                        <p><strong>Due:</strong> ${dueDate.toLocaleString()} ${isOverdue ? '<span style="color: red;">(Overdue)</span>' : ''}</p>
                        <p><strong>Points:</strong> ${q.totalPoints}</p>
                        ${q.score !== null ? `<p><strong>Your Score:</strong> ${q.score}/${q.totalPoints}</p>` : ''}
                    </div>
                    <div class="quiz-actions">
                        <button class="btn-action btn-download" onclick="downloadQuizFile('${q.filePath}')">Download Quiz</button>
                        ${status === 'pending' ? `<button class="btn-action btn-submit" onclick="openSubmitModal(${q.quizId}, '${q.title}')">Submit Answer</button>` : ''}
                        ${status === 'graded' ? `<button class="btn-action btn-view-score" onclick="viewScore(${q.quizId})">View Score Details</button>` : ''}
                    </div>
                </div>
            `;
        }).join('');
    } catch (error) {
        console.error('Error loading quizzes:', error);
    }
}

function downloadQuizFile(filePath) {
    // Files are in wwwroot, so they are accessed directly from the root URL
    window.open(`/${filePath}`, '_blank');
}

// ========== SUBMIT ANSWER ==========

function openSubmitModal(quizId, quizTitle) {
    document.getElementById('quizId').value = quizId;
    document.getElementById('quizNameSubmit').textContent = quizTitle;
    document.getElementById('submitForm').reset();
    document.getElementById('quizId').value = quizId;
    document.getElementById('submitModal').classList.add('show');
}

function closeSubmitModal() {
    document.getElementById('submitModal').classList.remove('show');
}

document.getElementById('submitForm').addEventListener('submit', async function (e) {
    e.preventDefault();

    const quizId = document.getElementById('quizId').value;
    const file = document.getElementById('answerFile').files[0];

    if (!file) {
        alert('Please select a file to upload');
        return;
    }

    const formData = new FormData();
    formData.append('quizId', quizId);
    formData.append('file', file);

    try {
        const response = await fetch(`${API_BASE}/student/submissions`, {
            method: 'POST',
            headers: {
                'Authorization': `Bearer ${token}`
            },
            body: formData
        });

        if (response.ok) {
            closeSubmitModal();
            loadQuizzes(currentCourseId);
            alert('Answer submitted successfully!');
        } else {
            const error = await response.json();
            alert(error.message || 'Error submitting answer');
        }
    } catch (error) {
        console.error('Error submitting answer:', error);
        alert('Error submitting answer');
    }
});

// ========== VIEW SCORE ==========

async function viewScore(quizId) {
    try {
        const response = await fetch(`${API_BASE}/student/quizzes/${quizId}/score`, {
            headers: { 'Authorization': `Bearer ${token}` }
        });
        const data = await response.json();

        const content = document.getElementById('scoreContent');
        content.innerHTML = `
            <div class="score-display">
                <h4>${data.quizTitle}</h4>
                <div class="score-value">${data.score} / ${data.totalPoints}</div>
                <p><strong>Percentage:</strong> ${((data.score / data.totalPoints) * 100).toFixed(2)}%</p>
                <p><strong>Submitted:</strong> ${new Date(data.submittedDate).toLocaleString()}</p>
                ${data.feedback ? `<p class="feedback"><strong>Feedback:</strong><br>${data.feedback}</p>` : ''}
            </div>
        `;

        document.getElementById('scoreModal').classList.add('show');
    } catch (error) {
        console.error('Error loading score:', error);
        alert('Error loading score details');
    }
}

function closeScoreModal() {
    document.getElementById('scoreModal').classList.remove('show');
}

// Load initial data
loadCourses();