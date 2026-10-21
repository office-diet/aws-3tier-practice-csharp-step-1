async function countUp() {
  try {
    const res = await fetch("/api/counter", { method: "POST" });
    if (!res.ok) throw new Error(`HTTP ${res.status}`);
    const data = await res.json();
    document.getElementById("count").textContent = data.count;
    document.getElementById("id").textContent = data.id;
  } catch (e) {
    document.getElementById("error").textContent = "API通信に失敗しました: " + e.message;
  }
}
countUp();

document.getElementById("list").addEventListener("click", async function(){
  const res = await fetch("/api/counter", {method: "GET"});
  if (!res.ok) return;
  const data = await res.json();
  let html = "";
  data.forEach(function(row){
    html += `${row.id}, ${row.accessedAt}<br>`;
  });
  document.getElementById("data").innerHTML = html;
});