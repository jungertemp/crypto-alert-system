import { useEffect, useState } from "react";
import {
  LineChart,
  Line,
  XAxis,
  YAxis,
  Tooltip,
  ResponsiveContainer,
  CartesianGrid,
} from "recharts";

type PricePoint = {
  price: number;
  capturedAt: string;
};

// ⚠️ CHANGE THIS PORT
const API_URL = "http://localhost:8080";

function App() {
  const [data, setData] = useState<PricePoint[]>([]);
  const [assetId, setAssetId] = useState("ethereum");
  const [loading, setLoading] = useState(false);

  async function loadData() {
    setLoading(true);

    try {
      const res = await fetch(
        `${API_URL}/api/prices/history?assetId=${assetId}&hours=24`
      );

      const json = await res.json();
      setData(json);
    } catch (e) {
      console.error("Error loading data", e);
    } finally {
      setLoading(false);
    }
  }

  useEffect(() => {
  loadData();

  const interval = setInterval(loadData, 10000);

  return () => clearInterval(interval);
}, [assetId]);

  const formatted = data.map((d) => ({
    ...d,
    time: new Date(d.capturedAt).toLocaleTimeString(),
  }));

  const currentPrice =
    data.length > 0 ? data[data.length - 1].price : null;

  return (
    <div style={{ padding: 24 }}>
      <h1>Crypto Alerts</h1>

      <div style={{ marginBottom: 16 }}>
        <input
          value={assetId}
          onChange={(e) => setAssetId(e.target.value)}
          placeholder="ethereum"
        />
        <button onClick={loadData}>Load</button>
      </div>

      <div style={{ marginBottom: 16 }}>
        Current price:{" "}
        {currentPrice ? `${currentPrice} USD` : "no data"}
      </div>

      {loading && <div>Loading...</div>}

      <div style={{ width: "100%", height: 300 }}>
        <ResponsiveContainer>
          <LineChart data={formatted}>
            <CartesianGrid strokeDasharray="3 3" />
            <XAxis dataKey="time" />
            <YAxis />
            <Tooltip />
            <Line type="monotone" dataKey="price" dot={false} />
          </LineChart>
        </ResponsiveContainer>
      </div>
    </div>
  );
}

export default App;