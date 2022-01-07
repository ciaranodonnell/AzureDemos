import * as asb from "@azure/service-bus"
import { promisify } from "util";
const connectionString = "Endpoint=sb://ciaransyoutubedemos.servicebus.windows.net/;SharedAccessKeyName=SendDemo;SharedAccessKey=";

const serviceBus = new asb.ServiceBusClient(connectionString);

const sender = serviceBus.createSender("demoqueue");

await sender.sendMessages({
    body: "Hello, world!",
    applicationProperties: { "my-property": "my-value" },
});
console.log("Message sent");

await sender.close();


const receiver = serviceBus.createReceiver("demotopic", "subscription1");

console.log("Waiting for messages...");

// const messages = await receiver.receiveMessages(5, { maxWaitTimeInMs: 10000 });
// for (const message of messages) {
//     console.log(`Message Received: ${message.body}`);
//     await receiver.completeMessage(message);
// }

await receiver.subscribe({
    async processMessage(message: asb.ServiceBusReceivedMessage): Promise<void> {
        console.log(`Process Message: ${message.body}`);
        //await receiver.completeMessage(message);
    },
    async processError(args: asb.ProcessErrorArgs): Promise<void> {
        console.log(args.error);
    }
}, { maxConcurrentCalls: 1, autoCompleteMessages: true });

/*
const delay = promisify(setTimeout);
await delay(10000);

await receiver.close();

console.log("Process Complete");
await serviceBus.close();
process.exit(0);
*/