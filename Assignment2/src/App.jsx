import UsedCarsListing from "./components/UserCarListing";
import store from "../redux/store";
import { Provider } from "react-redux";

function App() {
  return (
    <>
      <Provider store={store}>
        <UsedCarsListing />
      </Provider>
    </>
  );
}

export default App;
