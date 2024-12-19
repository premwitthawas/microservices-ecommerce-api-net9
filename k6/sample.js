import http from "k6/http";
import { sleep, check } from "k6";
export const options = {
  stages: [
    { duration: "10s", target: 30 }, // เริ่มจาก 50 VUs
    { duration: "1s", target: 0 }, // ลดเหลือ 0 VUs
  ],
  // vus: 10,
  // duration: "1s"
};
export default function () {
  const res = http.get("http://localhost:4002/api/orders");
  check(res, { "status was 200": (r) => r.status == 200 });
  sleep(0.1);
}
