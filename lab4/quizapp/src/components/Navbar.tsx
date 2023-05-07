import React from "react";
import { User } from "../models/user";
import { agent } from "../api/agent";
import "../css/navbar.css";
type Props = {
  user: User;
  setUser: React.Dispatch<React.SetStateAction<User>>;
  handleLogOut: () => Promise<void>;
};

function Navbar({ user, setUser, handleLogOut }: Props) {
  const clickedLogOut = async () => {
    await agent.Users.delete(user!.id);
    setUser(null);
    await handleLogOut();
    localStorage.removeItem("user");
    window.location.reload();
  };
  return user != null ? (
    <div>
      <button
        className="logout"
        onClick={() => clickedLogOut()}
        id="logoutButton"
      >
        Log Out
      </button>
    </div>
  ) : null;
}
export default Navbar;
