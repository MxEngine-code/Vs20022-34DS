import tkinter as tk

class AIBotUI:
    def __init__(self, root):
        self.root = root
        self.root.title("AI Bot")
        self.root.geometry("500x600")
        self.root.configure(bg="#0f172a")  # dark background

        # Header
        header = tk.Label(
            root,
            text="AI Assistant",
            bg="#0f172a",
            fg="#38bdf8",
            font=("Segoe UI", 16, "bold"),
            pady=10
        )
        header.pack()

        # Chat container
        self.chat_frame = tk.Frame(root, bg="#0f172a")
        self.chat_frame.pack(fill="both", expand=True, padx=10)

        # Entry box
        self.entry = tk.Entry(
            root,
            font=("Segoe UI", 12),
            bg="#1e293b",
            fg="white",
            insertbackground="white",
            relief="flat"
        )
        self.entry.pack(fill="x", padx=10, pady=10, ipady=8)
        self.entry.bind("<Return>", self.send_message)

        # Start animation
        self.root.after(500, lambda: self.type_message("Hello 👋 I'm your AI bot."))

    def create_bubble(self, text, sender="bot"):
        bubble_frame = tk.Frame(self.chat_frame, bg="#0f172a")
        bubble_frame.pack(anchor="w" if sender == "bot" else "e", pady=5)

        color = "#1e293b" if sender == "bot" else "#38bdf8"
        fg = "white"

        label = tk.Label(
            bubble_frame,
            text=text,
            bg=color,
            fg=fg,
            wraplength=300,
            justify="left",
            font=("Segoe UI", 11),
            padx=10,
            pady=6
        )
        label.pack()

        return label

    def type_message(self, text):
        label = self.create_bubble("", sender="bot")
        self.animate_text(label, text, 0)

    def animate_text(self, label, text, index):
        if index <= len(text):
            label.config(text=text[:index])
            self.root.after(25, lambda: self.animate_text(label, text, index + 1))

    def send_message(self, event=None):
        user_text = self.entry.get().strip()
        if not user_text:
            return

        self.create_bubble(user_text, sender="user")
        self.entry.delete(0, tk.END)

        response = self.generate_response(user_text)
        self.root.after(400, lambda: self.type_message(response))

    def generate_response(self, text):
        text = text.lower()
        if "hello" in text:
            return "Olá seja bem-vindo"
        elif "Como vc está" in text:
            return "Eu estou bem e vc?"
        else:
            return "Que interessante me conte mais."

root = tk.Tk()
app = AIBotUI(root)
root.mainloop()