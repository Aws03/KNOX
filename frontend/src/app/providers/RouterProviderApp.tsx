import { RouterProvider } from "react-router-dom";
import { router } from "../../lib/router/router";

export const RouterProviderApp = () => {
  return <RouterProvider router={router} />;
};
