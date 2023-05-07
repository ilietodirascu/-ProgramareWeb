import axios from "axios";
import React, { useEffect, useState } from "react";
import { TokenResponse } from "../models/tokenResponse";
import { agent } from "../api/agent";
import { CreateUser } from "../models/createUser";
import { Formik, Form, Field, ErrorMessage } from "formik";
import { Label, Button } from "semantic-ui-react";
import { User } from "../models/user";
import "../css/login.css";
type Props = {
  setUser: React.Dispatch<React.SetStateAction<User>>;
};
const Login = ({ setUser }: Props) => {
  const [isLoading, setIsLoading] = useState(false);
  useEffect(() => {
    initToken();
  });

  const config = {
    headers: {
      "X-Developer-Key": process.env.REACT_APP_API_KEY,
      "X-Developer-Secret": process.env.REACT_APP_API_SECRET,
    },
  };
  const onSubmit = async () => {
    setIsLoading(true);
    const firstName = document.getElementById("name") as HTMLInputElement;
    const lastName = document.getElementById("surname") as HTMLInputElement;
    const createUser: CreateUser = {
      name: firstName.value,
      surname: lastName.value,
    };
    const user = await agent.Users.create(createUser);
    localStorage.setItem("user", JSON.stringify(user));
    setIsLoading(false);
    setUser(user);
  };
  const createToken = async () => {
    var response = await axios.post<TokenResponse>(
      `${process.env.REACT_APP_API_URL}developers/v72/tokens`,
      null,
      config
    );
    return response.data;
  };
  const initToken = async () => {
    var token = localStorage.getItem("x-access-token");
    if (token === null) {
      var response = await createToken();
      console.log(response.token);
      localStorage.setItem("x-access-token", response.token);
    }
  };
  const validateField = (value: string) => {
    let error;
    if (value.length < 4 || value.length > 10) {
      error = "Min length 4 max length 10";
    }
    return error;
  };
  return (
    <div className="page-container centered-container">
      <div className="container">
        {!isLoading && (
          <Formik
            initialValues={{ name: "", surname: "", error: null }}
            onSubmit={(values, { setErrors }) => onSubmit()}
          >
            {({ handleSubmit, isSubmitting, errors, isValidating }) => (
              <Form
                className="ui form"
                onSubmit={handleSubmit}
                autoComplete="off"
              >
                <label htmlFor="name">Name</label>
                <Field
                  validate={validateField}
                  id="name"
                  name="name"
                  placeholder="Your Name"
                  type="name"
                />
                <label htmlFor="surname">Surname</label>
                <Field
                  id="surname"
                  name="surname"
                  type="surname"
                  placeholder="Your surname"
                />
                <ErrorMessage
                  name="error"
                  render={() => (
                    <Label
                      style={{ margin: 10 }}
                      basic
                      color="red"
                      content={errors.error}
                    />
                  )}
                />
                <Button
                  loading={isSubmitting}
                  positive
                  id="loginButton"
                  content="Login"
                  type="submit"
                  fluid
                ></Button>
              </Form>
            )}
          </Formik>
        )}
      </div>
    </div>
  );
};
export default Login;
