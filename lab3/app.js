"use strict";
var todos = JSON.parse(localStorage.getItem("todos")) || [];

window.addEventListener("load", () => {
  InjectElements();
  const form = document.querySelector("#new-task-form");
  const input = document.querySelector("#new-task-input");
  form.addEventListener("submit", (e) => {
    e.preventDefault();
    const task = input.value;
    if (!task) {
      alert("Please fill out the task");
      return;
    }
    todos.push({ id: Date.now(), name: task, done: false });
    localStorage.setItem("todos", JSON.stringify(todos));
    InjectElements(input);
    input.value = "";
  });
});

function InjectElements() {
  const list_el = document.querySelector("#tasks");
  const done_tasks = document.querySelector("#done_tasks");

  list_el.innerHTML = "";
  done_tasks.innerHTML = "";
  if (!todos) return;
  todos.map((x) => {
    const task_el = document.createRange().createContextualFragment(`
      <div data-id="${x.id}" class="task">
            <div class="content">
              <input type="text" class="text" value="${x.name}" readonly />
            </div>
            <div class="actions">
              <input type="checkbox" id="checkbox" class="checkbox" ${
                x.done ? "checked" : ""
              }>
              <label for="checkbox" Done </label>
              <button class="edit">Edit</button>
              <button class="delete">Delete</button>
            </div>
          </div>
    `);
    const edit_button = task_el.querySelector(".edit");
    const delete_button = task_el.querySelector(".delete");
    const checkbox = task_el.querySelector(".checkbox");
    const text_input = task_el.querySelector(".text");
    if (checkbox.checked) {
      done_tasks.appendChild(task_el);
    } else {
      list_el.appendChild(task_el);
    }
    edit_button.addEventListener("click", (e) => {
      const parent = e.target.closest(".task");
      const parent_id = parent.dataset.id;
      if (edit_button.innerText.toLowerCase() == "edit") {
        text_input.removeAttribute("readonly");
        text_input.focus();
        edit_button.innerText = "Save";
      } else {
        text_input.setAttribute("readonly", "readonly");
        todos = todos.map((x) =>
          x.id == parent_id
            ? { id: x.id, name: text_input.value, done: x.done }
            : x
        );
        localStorage.setItem("todos", JSON.stringify(todos));
        edit_button.innerText = "Edit";
      }
    });
    delete_button.addEventListener("click", (e) => {
      const parent_id = e.target.closest(".task").dataset.id;
      todos = todos.filter((x) => x.id != parent_id);
      localStorage.setItem("todos", JSON.stringify(todos));
      InjectElements();
    });
    checkbox.addEventListener("change", (e) => {
      const parent_id = +e.target.closest(".task").dataset.id;
      todos = todos.map((x) =>
        x.id === parent_id ? { id: x.id, name: x.name, done: !x.done } : x
      );
      localStorage.setItem("todos", JSON.stringify(todos));
      InjectElements();
    });
  });
}
