import React, { useEffect, useState } from "react";
import { User } from "../models/user";
import QuizList from "./QuizList";
import Login from "./Login";
const HomePage = () => {
  useEffect(() => {
    const userString = localStorage.getItem("user");
    if (userString != null) {
      setUser(JSON.parse(userString));
    }
  }, []);
  const [user, setUser] = useState<User>(null);
  return user != null ? <QuizList /> : <Login setUser={setUser} />;
};
export default HomePage;
