var appConfig =
{
  Authority: "https://logicidentityprod.b2clogin.com/tfp/logicidentityprod.onmicrosoft.com/B2C_1A_signup_signin/v2.0",
  ClientId: "25d9efa3-9ec1-4e2a-805d-ca5fefee7731",
  RedirectUri: "https://localhost:44326/authentication/login-callback",
  PostLogoutRedirectUri: "https://localhost:44326/authentication/logout-callback",
  ResponseType: "id_token token",
  Scope: "openid https://logicidentityprod.onmicrosoft.com/ieapis/todos.read https://logicidentityprod.onmicrosoft.com/ieapis/todos.write https://logicidentityprod.onmicrosoft.com/ieapis/dates.read",
  DatesApiUrl: "https://localhost:44327/",
  TodosApiUrl: "https://localhost:44328/"
};