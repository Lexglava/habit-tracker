const api = {
  habits: "/api/habits",
  stats: "/api/habits/stats",
  weeklyActivity: "/api/habits/activity/weekly"
};

const metric = {
  total: document.getElementById("mTotal"),
  last7: document.getElementById("mLast7"),
  avg: document.getElementById("mAvg"),
  best: document.getElementById("mBest")
};

const habitList = document.getElementById("habitList");
const chart = document.getElementById("chart");
const okMsg = document.getElementById("okMsg");
const errMsg = document.getElementById("errMsg");

function setMessage(ok, err) {
  okMsg.textContent = ok || "";
  errMsg.textContent = err || "";
}

async function fetchJson(url, options) {
  const response = await fetch(url, options);
  if (!response.ok) {
    let message = "Ошибка запроса";
    try {
      const body = await response.json();
      message = body.message || message;
    } catch {
      // Ignore json parse errors for non-json response.
    }
    throw new Error(message);
  }
  if (response.status === 204) return null;
  return response.json();
}

function renderWeeklyActivity(data) {
  const max = Math.max(1, ...data.map(x => x.completed));
  chart.innerHTML = data.map(point => {
    const height = Math.max(12, Math.round((point.completed / max) * 130));
    return `
      <div class="bar-col">
        <div class="bar-value">${point.completed}</div>
        <div class="bar" style="height:${height}px"></div>
        <div class="bar-label">${point.day}</div>
      </div>
    `;
  }).join("");
}

function habitItemTemplate(item) {
  return `
    <article class="habit">
      <h3>${item.name}</h3>
      <div class="muted">Категория: ${item.category}</div>
      <div class="muted">Цель в неделю: ${item.weeklyTarget}</div>
      <div class="muted">Серия: ${item.currentStreak} | Выполнений: ${item.totalCompletions}</div>
      <div class="actions">
        <button onclick="completeHabit('${item.id}')">Отметить сегодня</button>
        <button class="btn-danger" onclick="deleteHabit('${item.id}')">Удалить</button>
      </div>
    </article>
  `;
}

async function loadStats() {
  const stats = await fetchJson(api.stats);
  metric.total.textContent = stats.totalHabits;
  metric.last7.textContent = stats.completedLast7Days;
  metric.avg.textContent = `${stats.avgCompletionRatePercent}%`;
  metric.best.textContent = stats.bestStreak;
}

async function loadHabits() {
  const habits = await fetchJson(api.habits);
  habitList.innerHTML = habits.length
    ? habits.map(habitItemTemplate).join("")
    : "<p class='muted'>Пока нет привычек. Добавь первую и начни трекать прогресс.</p>";
}

async function loadWeeklyActivity() {
  const weekly = await fetchJson(api.weeklyActivity);
  renderWeeklyActivity(weekly);
}

async function createHabit() {
  const name = document.getElementById("name").value.trim();
  const category = document.getElementById("category").value.trim();
  const weeklyTarget = Number(document.getElementById("weeklyTarget").value);

  if (!name || !category) {
    setMessage("", "Заполни название и категорию.");
    return;
  }

  try {
    await fetchJson(api.habits, {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify({ name, category, weeklyTarget })
    });
    setMessage("Привычка создана.", "");
    await refresh();
  } catch (error) {
    setMessage("", error.message);
  }
}

window.completeHabit = async function completeHabit(id) {
  try {
    const today = new Date().toISOString().slice(0, 10);
    await fetchJson(`${api.habits}/${id}/complete`, {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify({ completedOn: today })
    });
    setMessage("Отметка добавлена.", "");
    await refresh();
  } catch (error) {
    setMessage("", error.message);
  }
};

window.deleteHabit = async function deleteHabit(id) {
  try {
    await fetchJson(`${api.habits}/${id}`, { method: "DELETE" });
    setMessage("Привычка удалена.", "");
    await refresh();
  } catch (error) {
    setMessage("", error.message);
  }
};

async function refresh() {
  await Promise.all([loadStats(), loadHabits(), loadWeeklyActivity()]);
}

document.getElementById("createBtn").addEventListener("click", createHabit);
document.getElementById("refreshBtn").addEventListener("click", () => {
  refresh().catch(error => setMessage("", error.message));
});

refresh().catch(error => setMessage("", error.message));
