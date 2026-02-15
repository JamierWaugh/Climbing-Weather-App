import './App.css';
import { useState, useEffect } from "react";

function App() {
  const [rocks, setRocks] = useState<string[]>([]);
  const [postcode, setPostCode] = useState("");
  const [rockType, setType] = useState("");

  //Call /rocks to get cached rocks list
  useEffect(() => {
    fetch("http://localhost:5000/rocks") //Replace with actual backend url
    .then((res) => res.json())
    .then((data) => {
      setRocks(data);
      console.log(data);
    }).catch((err) => console.error("Failed to load rocks", err));
  }, []);

  const handleSubmit = () => {
    if (!postcode || !rockType){
      alert("Please fill in both postcode and rock type");
      return;
    }
  }
  

return (
  
  <div> 
      <h1>caniclimbtoday</h1>

      <div>
      <input 
        type="text"
        value={postcode}
        onChange={(e) => setPostCode(e.target.value)}
      />
      </div>

    <br />
    
    <div>
        <select
          value={rockType}
          onChange={(e) => setType(e.target.value)}
        > 
        {/* Loops through all rocks in rock data into drop down menu */}
        {rocks.map((rock) => (
            <option value={rock}>
            {rock}
            </option>
        ))}
        </select>

        <br />
        <button onClick={handleSubmit}> Click to find out</button>
        
    </div>
    <p> Your post code is {postcode} </p>
    <p> Your rock type is {rockType} </p>

  </div>
  )
}

export default App
